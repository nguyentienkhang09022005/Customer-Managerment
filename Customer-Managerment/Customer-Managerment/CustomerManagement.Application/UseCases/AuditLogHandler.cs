using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using System.Text.Json;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class AuditLogHandler
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IMapper _mapper;

        public AuditLogHandler(
            IAuditLogRepository auditLogRepository,
            IStaffRepository staffRepository,
            IMapper mapper)
        {
            _auditLogRepository = auditLogRepository;
            _staffRepository = staffRepository;
            _mapper = mapper;
        }

        public async Task LogAsync(string action, string entityType, Guid entityId, object? oldValues, object? newValues, Guid? idStaff = null, string? ipAddress = null, string? userAgent = null, string? description = null)
        {
            var staffName = idStaff.HasValue ? await GetStaffNameAsync(idStaff.Value) : null;

            var auditLog = new AuditLog
            {
                Action = action.ToUpper(),
                EntityType = entityType,
                EntityId = entityId,
                OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
                IdStaff = idStaff,
                StaffName = staffName,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Description = description ?? GenerateDescription(action, entityType, entityId, staffName),
                Timestamp = DateTime.UtcNow
            };

            await _auditLogRepository.AddAsync(auditLog);
        }

        public async Task<AuditLogListResponse> GetAuditLogsAsync(string? entityType, Guid? entityId, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 50)
        {
            IQueryable<AuditLog> logs;

            if (entityType != null && entityId.HasValue)
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
            var totalCount = await _auditLogRepository.GetTotalCountAsync();

            return new AuditLogListResponse
            {
                Logs = _mapper.Map<List<AuditLogResponse>>(logList),
                TotalCount = totalCount
            };
        }

        public async Task<AuditLogListResponse> GetByStaffAsync(Guid idStaff, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 50)
        {
            var logs = await _auditLogRepository.GetByStaffAsync(idStaff);
            if (fromDate.HasValue && toDate.HasValue)
            {
                logs = logs.Where(l => l.Timestamp >= fromDate.Value && l.Timestamp <= toDate.Value);
            }

            var logList = logs.OrderByDescending(l => l.Timestamp).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalCount = logs.Count();

            return new AuditLogListResponse
            {
                Logs = _mapper.Map<List<AuditLogResponse>>(logList),
                TotalCount = totalCount
            };
        }

        public async Task<AuditLogListResponse> GetByActionAsync(string action, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 50)
        {
            var logs = await _auditLogRepository.GetByActionAsync(action);
            if (fromDate.HasValue && toDate.HasValue)
            {
                logs = logs.Where(l => l.Timestamp >= fromDate.Value && l.Timestamp <= toDate.Value);
            }

            var logList = logs.OrderByDescending(l => l.Timestamp).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalCount = logs.Count();

            return new AuditLogListResponse
            {
                Logs = _mapper.Map<List<AuditLogResponse>>(logList),
                TotalCount = totalCount
            };
        }

        public async Task<List<AuditLogResponse>> GetEntityHistoryAsync(string entityType, Guid entityId)
        {
            var logs = await _auditLogRepository.GetEntityHistoryAsync(entityType, entityId);
            var logList = logs.OrderByDescending(l => l.Timestamp).Take(100).ToList();
            return _mapper.Map<List<AuditLogResponse>>(logList);
        }

        public async Task<AuditStatisticsResponse> GetStatisticsAsync(DateTime fromDate, DateTime toDate)
        {
            return await _auditLogRepository.GetStatisticsAsync(fromDate, toDate);
        }

        private async Task<string?> GetStaffNameAsync(Guid idStaff)
        {
            var staff = await _staffRepository.GetStaffByIdAsync(idStaff);
            return staff?.Fullname;
        }

        private static string GenerateDescription(string action, string entityType, Guid entityId, string? staffName)
        {
            var staffPart = string.IsNullOrEmpty(staffName) ? "" : $"bởi '{staffName}'";
            return $"{action} {entityType} '{entityId}' {staffPart}".Trim();
        }
    }
}