using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class NotificationHandler
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationHandler(
            INotificationRepository notificationRepository,
            IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<NotificationResponse> GetNotificationByIdAsync(Guid idNotification)
        {
            var notification = await _notificationRepository.GetNotificationByIdAsync(idNotification);
            if (notification == null)
            {
                throw new NotificationNotFoundException();
            }
            return _mapper.Map<NotificationResponse>(notification);
        }

        public async Task<bool> MarkAsReadAsync(Guid idNotification)
        {
            return await _notificationRepository.MarkAsReadAsync(idNotification);
        }

        public async Task<bool> MarkAllAsReadAsync(Guid idStaff)
        {
            return await _notificationRepository.MarkAllAsReadAsync(idStaff);
        }

        public async Task<bool> PinNotificationAsync(Guid idNotification)
        {
            return await _notificationRepository.PinNotificationAsync(idNotification);
        }

        public async Task<bool> DeleteNotificationAsync(Guid idNotification)
        {
            return await _notificationRepository.SoftDeleteNotificationAsync(idNotification);
        }

        public async Task<int> GetUnreadCountAsync(Guid idStaff)
        {
            return await _notificationRepository.GetUnreadCountAsync(idStaff);
        }

        // Internal method for creating notifications
        public async Task<NotificationResponse> CreateNotificationAsync(Notification notification)
        {
            var created = await _notificationRepository.AddNotificationAsync(notification);
            return _mapper.Map<NotificationResponse>(created);
        }
    }
}