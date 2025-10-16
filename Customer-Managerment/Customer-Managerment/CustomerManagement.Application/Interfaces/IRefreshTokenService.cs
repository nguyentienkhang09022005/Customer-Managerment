namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task saveRefreshToken(string idUser, string refreshToken, TimeSpan duration);
        Task<string?> getRefreshToken(string idUser);
        Task deleteRefreshToken(string idUser);
    }
}
