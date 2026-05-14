using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<AuditLog?> GetByIdAsync(Guid idLog);
        Task<IQueryable<AuditLog>> GetAllAsync();
        Task<IQueryable<AuditLog>> GetByEntityAsync(string entityType, Guid entityId);
        Task<IQueryable<AuditLog>> GetByStaffAsync(Guid idStaff);
        Task<IQueryable<AuditLog>> GetByActionAsync(string action);
        Task<IQueryable<AuditLog>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IQueryable<AuditLog>> GetEntityHistoryAsync(string entityType, Guid entityId);
        Task<AuditLog> AddAsync(AuditLog auditLog);
        Task AddBatchAsync(IEnumerable<AuditLog> auditLogs);
        Task<int> GetTotalCountAsync();
        Task<AuditStatisticsResponse> GetStatisticsAsync(DateTime fromDate, DateTime toDate);
    }
}