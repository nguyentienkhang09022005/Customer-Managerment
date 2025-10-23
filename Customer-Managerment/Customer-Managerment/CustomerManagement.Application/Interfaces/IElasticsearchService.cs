using Customer_Managerment.CustomerManagement.Application.DTOs.Response;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IElasticsearchService
    {
        // Ghi hoặc cập nhật một user vào ES
        Task IndexUserAsync(UserResponse user);

        // Xóa một user khỏi ES
        Task DeleteUserAsync(Guid idUser);

        // Tìm kiếm user
        Task<List<UserResponse>> SearchUsersAsync(string keyword);
    }
}