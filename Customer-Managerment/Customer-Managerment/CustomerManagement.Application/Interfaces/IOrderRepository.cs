using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetListOrdersAsync(Guid idUser);

        Task<Order> AddOrderAsync(OrderDomain orderDomain);

        Task<Order> GetOrderByIdAsync(Guid idOrder);

        Task<Order> UpdateOrderAsync(OrderDomain orderDomain, Order order);

        Task DeleteOrderAsync(Guid idOrder);

        Task<Order> GetExistingOrderAsync(Guid idOrder);

        Task<bool> CheckOrderExistsAsync(Guid idOrder);
    }
}
