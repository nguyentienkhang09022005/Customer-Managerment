using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ITokenService
    {
        string generateAccessToken(UserDomain user);
        string generateRefreshToken(UserDomain user);
    }
}
