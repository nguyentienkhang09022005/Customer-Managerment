using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(Person user);
        string GenerateRefreshToken(Person user);
    }
}