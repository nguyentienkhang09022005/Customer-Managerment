using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IStaffRepository
    {
        Task<StaffDomain> AddStaffAsync(StaffDomain users);

        Task<StaffDomain?> GetStaffByIdAsync(Guid idStaff);

        Task<StaffDomain?> GetStaffByUsernameAsync(string userName);    

        Task<StaffDomain?> GetStaffByEmailAsync(string email);

        Task<List<StaffDomain>> GetListStaffAsync();

        Task<StaffDomain?> UpdateStaffAsync(StaffDomain staffDomain);

        Task<bool> CheckStaffExistsAsync(Guid idStaff);

        Task DeleteStaffAsync(Guid idStaff);
    }
}
