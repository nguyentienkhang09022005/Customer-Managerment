using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IStaffRepository
    {
        Task<Person> AddStaffAsync(Person staff);
        Task<Person?> GetStaffByIdAsync(Guid idStaff);
        Task<Person?> GetStaffByUsernameAsync(string userName);
        Task<Person?> GetStaffByEmailAsync(string email);
        Task<List<Person>> GetStaffByRoleAsync(string role);
        IQueryable<Person> GetStaffById(Guid idStaff);
        IQueryable<Person> GetListStaff();
        Task<Person?> UpdateStaffAsync(Person staff);
        Task<bool> CheckStaffExistsAsync(Guid idStaff);
        Task<bool> SoftDeleteStaffAsync(Guid idStaff);
        Task<bool> RestoreStaffAsync(Guid idStaff);
        Task<bool> UpdateStaffStatusAsync(Guid idStaff, int status);
        Task<bool> UpdateLastActiveAsync(Guid idStaff);
        Task<List<Person>> GetOnlineStaffsAsync();
        Task<List<Person>> GetStaffsByStatusAsync(int status);
    }
}