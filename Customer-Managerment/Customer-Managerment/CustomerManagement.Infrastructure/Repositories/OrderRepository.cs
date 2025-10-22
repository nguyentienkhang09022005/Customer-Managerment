using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly IOrderDetailRepository _orderDetailRepository;

        private static string DefaultOrderStatus = "Pending";

        public OrderRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, 
                               IMapper mapper, 
                               IOrderDetailRepository orderDetailRepository)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<Order> AddOrderAsync(OrderDomain orderDomain)
        {
            await using var context = _contextFactory.CreateDbContext();
            var order = _mapper.Map<Order>(orderDomain);

            order.IdOrder = Guid.NewGuid();
            order.Status = DefaultOrderStatus;
            order.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetListOrdersAsync(Guid idUser)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Orders
                .AsNoTracking()
                .Where(o => o.IdUser == idUser)
                .IgnoreAutoIncludes()
                .ToListAsync();
        }

        public async Task<OrderResponse> GetInfOrderAsync(Guid idOrder)
        {
            await using var context = _contextFactory.CreateDbContext();

            var order = await context.Orders
                .Include(o => o.IdUserNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.IdOrder == idOrder);

            if (order == null)
                throw new Exception("Không tìm thấy đơn hàng.");

            var orderDetails = await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(idOrder);

            var orderDomain = _mapper.Map<OrderDomain>(order);
            orderDomain.OrderDetailsDomain = _mapper.Map<List<OrderDetailDomain>>(orderDetails);

            var orderResponse = _mapper.Map<OrderResponse>(orderDomain);

            return orderResponse;
        }

        public async Task<Order> UpdateOrderAsync(OrderDomain orderDomain, Order order)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.Attach(order);
            _mapper.Map(orderDomain, order);
            context.Entry(order).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return order;
        }

        public async Task DeleteOrderAsync(Guid idOrder)
        {
            await using var context = _contextFactory.CreateDbContext();
            var order = await context.Orders.FindAsync(idOrder);
            if (order == null)
                throw new NotFoundException("Không tìm thấy đơn hàng!");
            context.Orders.Remove(order);
            await context.SaveChangesAsync();
        }

        public async Task<Order> GetExistingOrderAsync(Guid idOrder)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.IdOrder == idOrder);
        }

        public async Task<bool> CheckOrderExistsAsync(Guid idOrder)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Orders.AsNoTracking().AnyAsync(o => o.IdOrder == idOrder);
        }
    }
}
