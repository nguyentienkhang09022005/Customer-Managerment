using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IStaffActivityLogRepository
    {
        Task<StaffActivityLog?> GetByIdAsync(Guid idLog);
        Task<IQueryable<StaffActivityLog>> GetLogsByStaffAsync(Guid idStaff);
        Task<IQueryable<StaffActivityLog>> GetLogsByStaffAndDateRangeAsync(Guid idStaff, DateTime fromDate, DateTime toDate);
        Task<IQueryable<StaffActivityLog>> GetRecentLogsAsync(int count);
        Task<StaffActivityLog> AddAsync(StaffActivityLog log);
        Task AddBatchAsync(IEnumerable<StaffActivityLog> logs);
    }
}