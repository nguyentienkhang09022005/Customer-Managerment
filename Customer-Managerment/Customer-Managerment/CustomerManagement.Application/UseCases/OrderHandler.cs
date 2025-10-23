using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class OrderHandler
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;

        public OrderHandler(IOrderRepository orderRepository, 
                            IUserRepository userRepository, 
                            IProductRepository productRepository,
                            IOrderDetailRepository orderDetailRepository,
                            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
        }

        public async Task<List<OrderResponse>> GetListOrdersAsync(Guid idUser)
        {
            var list = await _orderRepository.GetListOrdersAsync(idUser);
            return _mapper.Map<List<OrderResponse>>(list);
        }

        public async Task<OrderResponse> GetInfOrderAsync(Guid idOrder)
        {
            return await _orderRepository.GetInfOrderAsync(idOrder);
        }

        public async Task<string> CreateOrderAsync(OrderCreationRequest orderCreationRequest)
        {
            // Lấy danh sách nhân viên
            var employees = await _userRepository.GetListEmployeesAsync();
            if (employees.Count == 0)
                throw new DomainException("Không có nhân viên nào trong hệ thống!", 400);

            // Chọn ngẫu nhiên 1 nhân viên
            var random = new Random();
            var employee = employees[random.Next(employees.Count)];

            var orderDomain = _mapper.Map<OrderDomain>(orderCreationRequest);
            orderDomain.IdUser = employee.IdUser;

            // Tạo chi tiết đơn hàng và tính tổng tiền
            decimal totalAmount = 0;
            var orderDetailsDomain = new List<OrderDetailDomain>();

            foreach (var productRequest in orderCreationRequest.orderDetailRequests)
            {
                var product = await _productRepository.GetProductByIdAsync(productRequest.IdProduct);
                if (product == null)
                    throw new DomainException($"Không tìm thấy sản phẩm có ID {productRequest.IdProduct}");

                var orderDetailDomain = new OrderDetailDomain
                {
                    IdOrderDetail = Guid.NewGuid(),
                    IdProduct = product.IdProduct,
                    Quantity = productRequest.Quantity,
                    UnitPrice = product.Price ?? 0,
                };
                orderDetailDomain.TotalPrice = (orderDetailDomain.Quantity ?? 0) * (orderDetailDomain.UnitPrice ?? 0);

                // Ràng buộc dữ liệu
                orderDetailDomain.Validate(); 
                totalAmount += orderDetailDomain.TotalPrice ?? 0;
                orderDetailsDomain.Add(orderDetailDomain);
            }

            orderDomain.TotalAmount = totalAmount;

            var createdOrder = await _orderRepository.AddOrderAsync(orderDomain);

            await _orderDetailRepository.AddOrderDetailsAsync(orderDetailsDomain, createdOrder.IdOrder);

            return "Tạo đơn hàng thành công!";
        }

        public async Task<string> DeleteOrderAsync(Guid idOrder)
        {
            await _orderRepository.DeleteOrderAsync(idOrder);
            return "Xóa đơn hàng thành công!";
        }
    }
}
