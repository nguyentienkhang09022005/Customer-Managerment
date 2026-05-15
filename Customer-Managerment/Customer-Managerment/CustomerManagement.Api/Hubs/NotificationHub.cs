using Microsoft.AspNetCore.SignalR;

namespace Customer_Managerment.CustomerManagement.Api.Hubs
{
    public class NotificationHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task JoinStaffGroup(Guid idStaff)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"staff_{idStaff}");
        }

        public async Task LeaveStaffGroup(Guid idStaff)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"staff_{idStaff}");
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}