using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<AuditLog?> GetByIdAsync(Guid idLog);
        Task<List<AuditLog>> GetAllAsync();
        Task<List<AuditLog>> GetByEntityAsync(string entityType, Guid entityId);
        Task<List<AuditLog>> GetByStaffAsync(Guid idStaff);
        Task<List<AuditLog>> GetByActionAsync(string action);
        Task<List<AuditLog>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<List<AuditLog>> GetEntityHistoryAsync(string entityType, Guid entityId);
        Task<AuditLog> AddAsync(AuditLog auditLog);
        Task AddBatchAsync(IEnumerable<AuditLog> auditLogs);
        Task<int> GetTotalCountAsync();
        Task<AuditStatisticsResponse> GetStatisticsAsync(DateTime fromDate, DateTime toDate);
    }
}