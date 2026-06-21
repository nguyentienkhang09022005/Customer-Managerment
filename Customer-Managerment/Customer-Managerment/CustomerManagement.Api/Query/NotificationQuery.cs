using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Microsoft.AspNetCore.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    [Authorize]
    public class NotificationQuery
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationQuery(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public NotificationResponse? GetNotificationById(Guid idNotification)
        {
            var notification = _notificationRepository.GetNotificationByIdAsync(idNotification).Result;
            return notification == null ? null : _mapper.Map<NotificationResponse>(notification);
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<NotificationResponse> GetNotifications(Guid idStaff)
        {
            var notifications = _notificationRepository.GetNotificationsByStaffAsync(idStaff).Result;
            return notifications.ProjectTo<NotificationResponse>(_mapper.ConfigurationProvider);
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<NotificationResponse> GetUnreadNotifications(Guid idStaff)
        {
            var notifications = _notificationRepository.GetUnreadNotificationsAsync(idStaff).Result;
            return notifications.ProjectTo<NotificationResponse>(_mapper.ConfigurationProvider);
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<NotificationResponse> GetPinnedNotifications(Guid idStaff)
        {
            var notifications = _notificationRepository.GetPinnedNotificationsAsync(idStaff).Result;
            return notifications.ProjectTo<NotificationResponse>(_mapper.ConfigurationProvider);
        }

        public int GetUnreadCount(Guid idStaff)
        {
            return _notificationRepository.GetUnreadCountAsync(idStaff).Result;
        }
    }
}