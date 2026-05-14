using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class StaffActivityLogRepository : IStaffActivityLogRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public StaffActivityLogRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<StaffActivityLog?> GetByIdAsync(Guid idLog)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.StaffActivityLogs
                .FirstOrDefaultAsync(l => l.IdLog == idLog);
        }

        public async Task<IQueryable<StaffActivityLog>> GetLogsByStaffAsync(Guid idStaff)
        {
            await using var context = _contextFactory.CreateDbContext();
            return context.StaffActivityLogs
                .Where(l => l.IdStaff == idStaff)
                .OrderByDescending(l => l.Timestamp)
                .AsNoTracking();
        }

        public async Task<IQueryable<StaffActivityLog>> GetLogsByStaffAndDateRangeAsync(Guid idStaff, DateTime fromDate, DateTime toDate)
        {
            await using var context = _contextFactory.CreateDbContext();
            return context.StaffActivityLogs
                .Where(l => l.IdStaff == idStaff && l.Timestamp >= fromDate && l.Timestamp <= toDate)
                .OrderByDescending(l => l.Timestamp)
                .AsNoTracking();
        }

        public async Task<IQueryable<StaffActivityLog>> GetRecentLogsAsync(int count)
        {
            await using var context = _contextFactory.CreateDbContext();
            return context.StaffActivityLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(count)
                .AsNoTracking();
        }

        public async Task<StaffActivityLog> AddAsync(StaffActivityLog log)
        {
            await using var context = _contextFactory.CreateDbContext();
            await context.StaffActivityLogs.AddAsync(log);
            await context.SaveChangesAsync();
            return log;
        }

        public async Task AddBatchAsync(IEnumerable<StaffActivityLog> logs)
        {
            await using var context = _contextFactory.CreateDbContext();
            await context.StaffActivityLogs.AddRangeAsync(logs);
            await context.SaveChangesAsync();
        }
    }
}