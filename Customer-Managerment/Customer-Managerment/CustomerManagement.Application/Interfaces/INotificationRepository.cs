using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification?> GetNotificationByIdAsync(Guid idNotification);
        Task<IQueryable<Notification>> GetNotificationsByStaffAsync(Guid idStaff);
        Task<IQueryable<Notification>> GetUnreadNotificationsAsync(Guid idStaff);
        Task<IQueryable<Notification>> GetPinnedNotificationsAsync(Guid idStaff);
        Task<int> GetUnreadCountAsync(Guid idStaff);
        Task<Notification> AddNotificationAsync(Notification notification);
        Task<Notification> UpdateNotificationAsync(Notification notification);
        Task<bool> MarkAsReadAsync(Guid idNotification);
        Task<bool> MarkAllAsReadAsync(Guid idStaff);
        Task<bool> SoftDeleteNotificationAsync(Guid idNotification);
        Task<bool> PinNotificationAsync(Guid idNotification);
        Task<(List<Notification> Items, int TotalCount)> GetNotificationsPagedByStaffAsync(Guid idStaff, int page, int pageSize);
    }
}