using Microsoft.AspNetCore.SignalR;
using Customer_Managerment.CustomerManagement.Api.Hubs;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;

namespace Customer_Managerment.CustomerManagement.Api.Services
{
    public interface IRealtimeNotificationService
    {
        Task SendNotificationToStaffAsync(Guid idStaff, NotificationResponse notification);
        Task BroadcastNotificationAsync(NotificationResponse notification);
    }

    public class RealtimeNotificationService : IRealtimeNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public RealtimeNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationToStaffAsync(Guid idStaff, NotificationResponse notification)
        {
            await _hubContext.Clients.Group($"staff_{idStaff}").SendAsync("ReceiveNotification", notification);
        }

        public async Task BroadcastNotificationAsync(NotificationResponse notification)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
        }
    }
}