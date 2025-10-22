using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases.OrderHandler
{
    public class OrderHandler
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public OrderHandler(IOrderRepository orderRepository, IUserRepository userRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<OrderResponse>> GetListOrdersAsync(Guid idUser)
        {
            var list = await _orderRepository.GetListOrdersAsync(idUser);
            return _mapper.Map<List<OrderResponse>>(list);
        }

        public async Task<OrderResponse> GetInfOrderAsync(Guid idOrder)
        {
            var order = await _orderRepository.GetOrderByIdAsync(idOrder);
            return _mapper.Map<OrderResponse>(order);
        }

        public async Task<OrderResponse> CreateOrderAsync(OrderCreationRequest request)
        {
            bool userExists = await _userRepository.CheckUserExistsAsync(request.IdUser);
            if (!userExists)
                throw new DomainException("Nhân viên không tồn tại!", 404);

            bool customerExists = await _userRepository.CheckUserExistsAsync(request.IdUser);
            if (!customerExists)
                throw new DomainException("Khách hàng không tồn tại!", 404);

            var domain = _mapper.Map<OrderDomain>(request);
            var newOrder = await _orderRepository.AddOrderAsync(domain);
            return _mapper.Map<OrderResponse>(newOrder);
        }

        public async Task<OrderResponse> UpdateOrderAsync(OrderUpdateRequest request, Guid idOrder)
        {
            var orderEntity = await _orderRepository.GetExistingOrderAsync(idOrder);
            if (orderEntity == null)
                throw new DomainException("Không tìm thấy đơn hàng cần cập nhật!", 404);

            var domain = _mapper.Map<OrderDomain>(orderEntity);
            _mapper.Map(request, domain);

            var updated = await _orderRepository.UpdateOrderAsync(domain, orderEntity);
            return _mapper.Map<OrderResponse>(updated);
        }

        public async Task<string> DeleteOrderAsync(Guid idOrder)
        {
            await _orderRepository.DeleteOrderAsync(idOrder);
            return "Xóa đơn hàng thành công!";
        }
    }
}
