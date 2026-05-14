using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class NotificationMutation
    {
        private readonly NotificationHandler _notificationHandler;

        public NotificationMutation(NotificationHandler notificationHandler)
        {
            _notificationHandler = notificationHandler;
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
    }
}