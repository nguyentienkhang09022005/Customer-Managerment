using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Person> AddCustomerAsync(Person customer);
        Task<Person?> GetCustomerByIdAsync(Guid idCustomer);
        IQueryable<Person> GetListCustomer();
        Task<List<Person>> GetListCustomerAsync();
        IQueryable<Person> GetCustomerById(Guid idCustomer);
        Task<Person?> UpdateCustomerAsync(Person customer);
        Task<bool> CheckCustomerExistsAsync(Guid idCustomer);
        Task<Person?> GetCustomerByEmailAsync(string email);
        Task<int> GetTotalCustomersAsync();
        Task<bool> SoftDeleteCustomerAsync(Guid idCustomer);
        Task<bool> RestoreCustomerAsync(Guid idCustomer);
    }
}