using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public NotificationRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<Notification?> GetNotificationByIdAsync(Guid idNotification)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Notifications
                .Include(n => n.IdStaffNavigation)
                .FirstOrDefaultAsync(n => n.IdNotification == idNotification);
        }

        public async Task<IQueryable<Notification>> GetNotificationsByStaffAsync(Guid idStaff)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Notifications
                .Include(n => n.IdStaffNavigation)
                .Where(n => n.IdStaff == idStaff && !n.IsRead || n.IsPinned)
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking();
        }

        public async Task<IQueryable<Notification>> GetUnreadNotificationsAsync(Guid idStaff)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Notifications
                .Where(n => n.IdStaff == idStaff && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking();
        }

        public async Task<IQueryable<Notification>> GetPinnedNotificationsAsync(Guid idStaff)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Notifications
                .Where(n => n.IdStaff == idStaff && n.IsPinned)
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking();
        }

        public async Task<int> GetUnreadCountAsync(Guid idStaff)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Notifications.CountAsync(n => n.IdStaff == idStaff && !n.IsRead);
        }

        public async Task<Notification> AddNotificationAsync(Notification notification)
        {
            await using var context = _contextFactory.CreateDbContext();

            notification.IdNotification = Guid.NewGuid();
            notification.CreatedAt = DateTime.UtcNow;

            await context.Notifications.AddAsync(notification);
            await context.SaveChangesAsync();
            return notification;
        }

        public async Task<Notification> UpdateNotificationAsync(Notification notification)
        {
            await using var context = _contextFactory.CreateDbContext();
            var existing = await context.Notifications.FirstOrDefaultAsync(n => n.IdNotification == notification.IdNotification);

            if (existing == null)
                throw new NotFoundException("Không tìm thấy Notification!");

            existing.IsRead = notification.IsRead;
            existing.IsPinned = notification.IsPinned;

            await context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> MarkAsReadAsync(Guid idNotification)
        {
            await using var context = _contextFactory.CreateDbContext();
            var rows = await context.Notifications
                .Where(n => n.IdNotification == idNotification)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
            return rows > 0;
        }

        public async Task<bool> MarkAllAsReadAsync(Guid idStaff)
        {
            await using var context = _contextFactory.CreateDbContext();
            var rows = await context.Notifications
                .Where(n => n.IdStaff == idStaff && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
            return rows > 0;
        }

        public async Task<bool> SoftDeleteNotificationAsync(Guid idNotification)
        {
            await using var context = _contextFactory.CreateDbContext();
            var rows = await context.Notifications
                .Where(n => n.IdNotification == idNotification)
                .ExecuteDeleteAsync();
            return rows > 0;
        }

        public async Task<bool> PinNotificationAsync(Guid idNotification)
        {
            await using var context = _contextFactory.CreateDbContext();
            var rows = await context.Notifications
                .Where(n => n.IdNotification == idNotification)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsPinned, n => !n.IsPinned));
            return rows > 0;
        }
    }
}