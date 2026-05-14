using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IPersonRepository
    {
        // Staff operations (Discriminator = Staff)
        Task<Person> AddStaffAsync(Person staff);
        Task<Person?> GetStaffByIdAsync(Guid idStaff);
        Task<Person?> GetStaffByUsernameAsync(string userName);
        Task<Person?> GetStaffByEmailAsync(string email);
        IQueryable<Person> GetStaffById(Guid idStaff);
        IQueryable<Person> GetListStaff();
        Task<Person?> UpdateStaffAsync(Person staff);
        Task<bool> CheckStaffExistsAsync(Guid idStaff);
        Task<bool> SoftDeleteStaffAsync(Guid idStaff, string deletedBy);
        Task<bool> RestoreStaffAsync(Guid idStaff);

        // Lead operations (Discriminator = Lead)
        Task<Person> AddLeadAsync(Person lead);
        Task<Person?> GetLeadByIdAsync(Guid idLead);
        IQueryable<Person> GetLeadById(Guid idLead);
        IQueryable<Person> GetListLead();
        Task<Person?> UpdateLeadAsync(Person lead);
        Task<bool> CheckLeadExistsAsync(Guid idLead);
        Task<bool> SoftDeleteLeadAsync(Guid idLead, string deletedBy);
        Task<bool> RestoreLeadAsync(Guid idLead);

        // Customer operations (Discriminator = Customer)
        Task<Person> AddCustomerAsync(Person customer);
        Task<Person?> GetCustomerByIdAsync(Guid idCustomer);
        IQueryable<Person> GetCustomerById(Guid idCustomer);
        IQueryable<Person> GetListCustomer();
        Task<Person?> UpdateCustomerAsync(Person customer);
        Task<bool> CheckCustomerExistsAsync(Guid idCustomer);
        Task<bool> SoftDeleteCustomerAsync(Guid idCustomer, string deletedBy);
        Task<bool> RestoreCustomerAsync(Guid idCustomer);
    }
}