using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public AuditLogRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<AuditLog?> GetByIdAsync(Guid idLog)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.AuditLogs.FirstOrDefaultAsync(a => a.IdLog == idLog);
        }

        public async Task<List<AuditLog>> GetAllAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByEntityAsync(string entityType, Guid entityId)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.AuditLogs
                .Where(a => a.EntityType == entityType && a.EntityId == entityId)
                .OrderByDescending(a => a.Timestamp)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByStaffAsync(Guid idStaff)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.AuditLogs
                .Where(a => a.IdStaff == idStaff)
                .OrderByDescending(a => a.Timestamp)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByActionAsync(string action)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.AuditLogs
                .Where(a => a.Action == action.ToUpper())
                .OrderByDescending(a => a.Timestamp)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.AuditLogs
                .Where(a => a.Timestamp >= fromDate && a.Timestamp <= toDate)
                .OrderByDescending(a => a.Timestamp)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetEntityHistoryAsync(string entityType, Guid entityId)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.AuditLogs
                .Where(a => a.EntityType == entityType && a.EntityId == entityId)
                .OrderByDescending(a => a.Timestamp)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<AuditLog> AddAsync(AuditLog auditLog)
        {
            await using var context = _contextFactory.CreateDbContext();
            await context.AuditLogs.AddAsync(auditLog);
            await context.SaveChangesAsync();
            return auditLog;
        }

        public async Task AddBatchAsync(IEnumerable<AuditLog> auditLogs)
        {
            await using var context = _contextFactory.CreateDbContext();
            await context.AuditLogs.AddRangeAsync(auditLogs);
            await context.SaveChangesAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.AuditLogs.CountAsync();
        }

        public async Task<AuditStatisticsResponse> GetStatisticsAsync(DateTime fromDate, DateTime toDate)
        {
            await using var context = _contextFactory.CreateDbContext();
            var logs = await context.AuditLogs
                .Where(a => a.Timestamp >= fromDate && a.Timestamp <= toDate)
                .AsNoTracking()
                .ToListAsync();

            return new AuditStatisticsResponse
            {
                TotalLogs = logs.Count,
                CreateCount = logs.Count(l => l.Action == "CREATE"),
                UpdateCount = logs.Count(l => l.Action == "UPDATE"),
                DeleteCount = logs.Count(l => l.Action == "DELETE"),
                RestoreCount = logs.Count(l => l.Action == "RESTORE"),
                EntityTypeCounts = logs.GroupBy(l => l.EntityType).ToDictionary(g => g.Key, g => g.Count()),
                ActionCounts = logs.GroupBy(l => l.Action).ToDictionary(g => g.Key, g => g.Count())
            };
        }
    }
}