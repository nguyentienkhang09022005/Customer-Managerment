using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ITokenService
    {
        string generateAccessToken(StaffDomain user);
        string generateRefreshToken(StaffDomain user);
    }
}
