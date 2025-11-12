using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ICustomerRepository
    {
        Task<CustomerDomain> AddCustomerAsync(CustomerDomain customerDomain);

        Task<CustomerDomain?> GetCustomerByIdAsync(Guid idCustomer);

        IQueryable<CustomerDomain> GetListCustomer();

        IQueryable<CustomerDomain> GetCustomerById(Guid idCustomer);

        Task<CustomerDomain?> UpdateCustomerAsync(CustomerDomain customerDomain);

        Task DeleteCustomerAsync(Guid idCustomer);

        Task<bool> CheckCustomerExistsAsync(Guid idCustomer);
    }
}
