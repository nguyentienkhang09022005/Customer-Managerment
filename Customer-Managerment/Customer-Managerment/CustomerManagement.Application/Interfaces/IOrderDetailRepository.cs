using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IOrderDetailRepository
    {
        Task AddOrderDetailsAsync(IEnumerable<OrderDetailDomain> orderDetailsDomain, Guid idOrder);
        Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(Guid idOrder);
    }
}
