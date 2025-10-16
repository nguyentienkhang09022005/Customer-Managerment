using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(UserDomain users);

        Task<UserDomain?> GetUserByIdAsync(Guid idUser);

        Task<UserDomain?> GetUserByEmailAsync(string email);
    }
}
