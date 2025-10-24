using Customer_Managerment.CustomerManagement.Application.DTOs.Response;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IElasticsearchService
    {
        // Ghi hoặc cập nhật một user vào ES
        Task IndexStaffAsync(StaffResponse staffResponse);

        // Xóa một staff khỏi ES
        Task DeleteStaffAsync(Guid idStaff);

        // Tìm kiếm staff
        Task<List<StaffResponse>> SearchStaffsAsync(string keyword);
    }
}