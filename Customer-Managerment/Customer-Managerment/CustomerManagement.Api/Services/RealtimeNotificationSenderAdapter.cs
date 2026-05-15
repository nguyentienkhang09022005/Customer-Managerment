using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Api.Services;

namespace Customer_Managerment.CustomerManagement.Api.Services
{
    public class RealtimeNotificationSenderAdapter : IRealtimeNotificationSender
    {
        private readonly IRealtimeNotificationService _realtimeService;

        public RealtimeNotificationSenderAdapter(IRealtimeNotificationService realtimeService)
        {
            _realtimeService = realtimeService;
        }

        public async Task SendNotificationToStaffAsync(Guid idStaff, NotificationResponse notification)
        {
            await _realtimeService.SendNotificationToStaffAsync(idStaff, notification);
        }
    }
}