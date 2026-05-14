namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task SaveRefreshTokenAsync(string idUser, string refreshToken, TimeSpan duration);
        Task<string?> GetRefreshTokenAsync(string idUser);
        Task DeleteRefreshTokenAsync(string idUser);
    }
}
