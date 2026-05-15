using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class AuditLogQuery
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IMapper _mapper;

        public AuditLogQuery(IAuditLogRepository auditLogRepository, IMapper mapper)
        {
            _auditLogRepository = auditLogRepository;
            _mapper = mapper;
        }

        [UseFiltering]
        [UseSorting]
        public async Task<List<AuditLogResponse>> GetAuditLogsAsync(
            string? entityType = null,
            Guid? entityId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 50)
        {
            List<AuditLog> logs;

            if (!string.IsNullOrEmpty(entityType) && entityId.HasValue)
            {
                logs = await _auditLogRepository.GetByEntityAsync(entityType, entityId.Value);
            }
            else if (fromDate.HasValue && toDate.HasValue)
            {
                logs = await _auditLogRepository.GetByDateRangeAsync(fromDate.Value, toDate.Value);
            }
            else
            {
                logs = await _auditLogRepository.GetAllAsync();
            }

            var logList = logs.OrderByDescending(l => l.Timestamp).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return _mapper.Map<List<AuditLogResponse>>(logList);
        }

        [UseFiltering]
        [UseSorting]
        public async Task<List<AuditLogResponse>> GetAuditLogsByStaffAsync(
            Guid idStaff,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 50)
        {
            var logs = await _auditLogRepository.GetByStaffAsync(idStaff);
            if (fromDate.HasValue && toDate.HasValue)
            {
                logs = logs.Where(l => l.Timestamp >= fromDate.Value && l.Timestamp <= toDate.Value).ToList();
            }

            var logList = logs.OrderByDescending(l => l.Timestamp).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return _mapper.Map<List<AuditLogResponse>>(logList);
        }

        [UseFiltering]
        [UseSorting]
        public async Task<List<AuditLogResponse>> GetAuditLogsByActionAsync(
            string action,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 50)
        {
            var logs = await _auditLogRepository.GetByActionAsync(action);
            if (fromDate.HasValue && toDate.HasValue)
            {
                logs = logs.Where(l => l.Timestamp >= fromDate.Value && l.Timestamp <= toDate.Value).ToList();
            }

            var logList = logs.OrderByDescending(l => l.Timestamp).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return _mapper.Map<List<AuditLogResponse>>(logList);
        }

        [UseFiltering]
        [UseSorting]
        public async Task<List<AuditLogResponse>> GetEntityHistoryAsync(string entityType, Guid entityId)
        {
            var logs = await _auditLogRepository.GetEntityHistoryAsync(entityType, entityId);
            var logList = logs.OrderByDescending(l => l.Timestamp).Take(100).ToList();
            return _mapper.Map<List<AuditLogResponse>>(logList);
        }

        public async Task<AuditStatisticsResponse> GetAuditStatisticsAsync(DateTime fromDate, DateTime toDate)
        {
            return await _auditLogRepository.GetStatisticsAsync(fromDate, toDate);
        }
    }
}