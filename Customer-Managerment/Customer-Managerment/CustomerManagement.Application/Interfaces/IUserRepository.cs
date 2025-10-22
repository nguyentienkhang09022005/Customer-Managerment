using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(UserDomain users);

        Task<UserDomain?> GetUserByIdAsync(Guid idUser);

        Task<UserDomain?> GetUserByUsernameAsync(string userName);

        Task<UserDomain?> GetUserByEmailAsync(string email);

        Task<List<User>> GetListUsersAsync();

        Task<List<User>> GetListEmployeesAsync();

        Task<List<User>> GetListCustomerAsync();

        Task<UserDomain?> UpdateUserAsync(UserDomain userDomain);

        Task<bool> CheckUserExistsAsync(Guid idUser);

        Task<UserDomain?> GetExistingUserAsync(Guid idUser);

        Task DeleteUserAsync(Guid idUser);
    }
}
