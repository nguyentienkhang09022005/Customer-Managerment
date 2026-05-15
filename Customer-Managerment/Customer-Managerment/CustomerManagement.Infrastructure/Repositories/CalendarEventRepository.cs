using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class CalendarEventRepository : ICalendarEventRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public CalendarEventRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<CalendarEvent?> GetByIdAsync(Guid idEvent)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.CalendarEvents
                .FirstOrDefaultAsync(e => e.IdEvent == idEvent);
        }

        public async Task<List<CalendarEvent>> GetAllAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.CalendarEvents
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.StartTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CalendarEvent>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.CalendarEvents
                .Where(e => !e.IsDeleted && e.StartTime >= fromDate && e.EndTime <= toDate)
                .OrderBy(e => e.StartTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CalendarEvent>> GetByStaffAsync(Guid idStaff)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.CalendarEvents
                .Where(e => !e.IsDeleted && e.IdStaff == idStaff)
                .OrderBy(e => e.StartTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CalendarEvent>> GetByEntityAsync(string entityType, Guid entityId)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.CalendarEvents
                .Where(e => !e.IsDeleted && e.RelatedEntityType == entityType && e.RelatedEntityId == entityId)
                .OrderBy(e => e.StartTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CalendarEvent>> GetUpcomingEventsAsync(Guid idStaff, int days)
        {
            await using var context = _contextFactory.CreateDbContext();
            var now = DateTime.UtcNow;
            var future = now.AddDays(days);
            return await context.CalendarEvents
                .Where(e => !e.IsDeleted && e.IdStaff == idStaff && e.StartTime >= now && e.StartTime <= future)
                .OrderBy(e => e.StartTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CalendarEvent> AddAsync(CalendarEvent calendarEvent)
        {
            await using var context = _contextFactory.CreateDbContext();
            await context.CalendarEvents.AddAsync(calendarEvent);
            await context.SaveChangesAsync();
            return calendarEvent;
        }

        public async Task<CalendarEvent> UpdateAsync(CalendarEvent calendarEvent)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.CalendarEvents.Update(calendarEvent);
            await context.SaveChangesAsync();
            return calendarEvent;
        }

        public async Task<bool> SoftDeleteAsync(Guid idEvent)
        {
            await using var context = _contextFactory.CreateDbContext();
            var calendarEvent = await context.CalendarEvents.FindAsync(idEvent);
            if (calendarEvent == null)
                return false;

            calendarEvent.IsDeleted = true;
            calendarEvent.DeletedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreAsync(Guid idEvent)
        {
            await using var context = _contextFactory.CreateDbContext();
            var calendarEvent = await context.CalendarEvents
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.IdEvent == idEvent && e.IsDeleted);
            if (calendarEvent == null)
                return false;

            calendarEvent.IsDeleted = false;
            calendarEvent.DeletedAt = null;

            await context.SaveChangesAsync();
            return true;
        }
    }
}