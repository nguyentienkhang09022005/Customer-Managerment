using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Customer_Managerment.CustomerManagement.Api.Services;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class NotificationMutation
    {
        private readonly NotificationHandler _notificationHandler;
        private readonly IRealtimeNotificationService _realtimeNotificationService;

        public NotificationMutation(
            NotificationHandler notificationHandler,
            IRealtimeNotificationService realtimeNotificationService)
        {
            _notificationHandler = notificationHandler;
            _realtimeNotificationService = realtimeNotificationService;
        }

        public async Task<bool> MarkAsReadAsync(Guid idNotification)
        {
            return await _notificationHandler.MarkAsReadAsync(idNotification);
        }

        public async Task<bool> MarkAllAsReadAsync(Guid idStaff)
        {
            return await _notificationHandler.MarkAllAsReadAsync(idStaff);
        }

        public async Task<bool> PinNotificationAsync(Guid idNotification)
        {
            return await _notificationHandler.PinNotificationAsync(idNotification);
        }

        public async Task<bool> DeleteNotificationAsync(Guid idNotification)
        {
            return await _notificationHandler.DeleteNotificationAsync(idNotification);
        }

        // Internal method for creating notifications with realtime support
        public async Task<NotificationResponse> CreateNotificationAsync(
            Guid idStaff,
            string title,
            string message,
            string type,
            string? relatedEntityType = null,
            Guid? relatedEntityId = null)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                IdStaff = idStaff,
                RelatedEntityType = relatedEntityType,
                RelatedEntityId = relatedEntityId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _notificationHandler.CreateNotificationAsync(notification);

            // Send realtime notification
            await _realtimeNotificationService.SendNotificationToStaffAsync(idStaff, created);

            return created;
        }
    }
}