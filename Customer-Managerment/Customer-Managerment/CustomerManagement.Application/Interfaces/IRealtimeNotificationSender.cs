using Customer_Managerment.CustomerManagement.Application.DTOs.Response;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IRealtimeNotificationSender
    {
        Task SendNotificationToStaffAsync(Guid idStaff, NotificationResponse notification);
    }
}