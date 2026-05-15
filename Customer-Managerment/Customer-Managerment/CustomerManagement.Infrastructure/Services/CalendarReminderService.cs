using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Services
{
    public class CalendarReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CalendarReminderService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

        public CalendarReminderService(IServiceProvider serviceProvider, ILogger<CalendarReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Calendar Reminder Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndSendRemindersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Calendar Reminder Service");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Calendar Reminder Service stopped");
        }

        private async Task CheckAndSendRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<CustomerManagementDbContext>>();
            var notificationRepo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

            await using var context = contextFactory.CreateDbContext();
            var now = DateTime.UtcNow;

            // Find events with reminders in the next 5-10 minutes
            var reminderWindowStart = now.AddMinutes(5);
            var reminderWindowEnd = now.AddMinutes(10);

            var eventsWithReminders = await context.CalendarEvents
                .Include(e => e.IdStaffNavigation)
                .Where(e => !e.IsDeleted
                    && e.Status == CalendarEventStatusConstant.FromString(CalendarEventStatusConstant.EventStatusScheduled)
                    && e.ReminderMinutes != null && e.ReminderMinutes > 0
                    && e.StartTime >= reminderWindowStart
                    && e.StartTime <= reminderWindowEnd)
                .ToListAsync();

            foreach (var evt in eventsWithReminders)
            {
                // Check if reminder was already sent (avoid duplicate)
                var existingReminder = await context.Notifications
                    .AnyAsync(n => n.RelatedEntityType == "CalendarEvent"
                        && n.RelatedEntityId == evt.IdEvent
                        && n.Type == NotificationTypeConstant.NotificationReminder
                        && n.CreatedAt > now.AddMinutes(-10));

                if (existingReminder)
                    continue;

                // Get participants
                var participants = await context.EventParticipants
                    .Where(p => p.IdEvent == evt.IdEvent)
                    .ToListAsync();

                // Send notification to organizer
                await SendReminderNotificationAsync(notificationRepo, evt, evt.IdStaffNavigation?.Fullname ?? "Unknown", "organizer");

                // Send notification to participants
                foreach (var participant in participants)
                {
                    var staff = await context.Persons.FirstOrDefaultAsync(p => p.Id == participant.IdStaff);
                    if (staff != null)
                    {
                        await SendReminderNotificationAsync(notificationRepo, evt, staff.Fullname, "participant");
                    }
                }

                _logger.LogInformation("Sent reminder for event: {EventId} - {Title}", evt.IdEvent, evt.Title);
            }
        }

        private async Task SendReminderNotificationAsync(
            INotificationRepository notificationRepo,
            CalendarEvent evt,
            string recipientName,
            string recipientType)
        {
            var notification = new Notification
            {
                Title = $"Nhắc nhở: {evt.Title}",
                Message = $"Sự kiện của bạn sẽ bắt đầu trong {evt.ReminderMinutes} phút. Loại: {CalendarEventTypeConstant.ToString(evt.EventType)}. Thời gian: {evt.StartTime:HH:mm}",
                Type = NotificationTypeConstant.NotificationReminder,
                IdStaff = evt.IdStaff,
                RelatedEntityType = "CalendarEvent",
                RelatedEntityId = evt.IdEvent,
                IsRead = false,
                IsPinned = false,
                CreatedAt = DateTime.UtcNow
            };

            if (recipientType == "participant")
            {
                notification.Title = $"Nhắc nhở tham gia: {evt.Title}";
            }

            await notificationRepo.AddNotificationAsync(notification);
        }
    }
}