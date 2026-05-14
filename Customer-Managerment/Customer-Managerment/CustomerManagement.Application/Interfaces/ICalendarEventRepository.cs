using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ICalendarEventRepository
    {
        Task<CalendarEvent?> GetByIdAsync(Guid idEvent);
        Task<IQueryable<CalendarEvent>> GetAllAsync();
        Task<IQueryable<CalendarEvent>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IQueryable<CalendarEvent>> GetByStaffAsync(Guid idStaff);
        Task<IQueryable<CalendarEvent>> GetByEntityAsync(string entityType, Guid entityId);
        Task<IQueryable<CalendarEvent>> GetUpcomingEventsAsync(Guid idStaff, int days);
        Task<CalendarEvent> AddAsync(CalendarEvent calendarEvent);
        Task<CalendarEvent> UpdateAsync(CalendarEvent calendarEvent);
        Task<bool> SoftDeleteAsync(Guid idEvent);
        Task<bool> RestoreAsync(Guid idEvent);
    }
}