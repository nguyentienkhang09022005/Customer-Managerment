using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ICalendarEventRepository
    {
        Task<CalendarEvent?> GetByIdAsync(Guid idEvent);
        Task<List<CalendarEvent>> GetAllAsync();
        Task<List<CalendarEvent>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<List<CalendarEvent>> GetByStaffAsync(Guid idStaff);
        Task<List<CalendarEvent>> GetByEntityAsync(string entityType, Guid entityId);
        Task<List<CalendarEvent>> GetUpcomingEventsAsync(Guid idStaff, int days);
        Task<CalendarEvent> AddAsync(CalendarEvent calendarEvent);
        Task<CalendarEvent> UpdateAsync(CalendarEvent calendarEvent);
        Task<bool> SoftDeleteAsync(Guid idEvent);
        Task<bool> RestoreAsync(Guid idEvent);
    }
}