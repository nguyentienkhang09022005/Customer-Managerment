using Microsoft.AspNetCore.SignalR;

namespace Customer_Managerment.CustomerManagement.Api.Hubs
{
    public class NoteHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task JoinEntityGroup(string entityType, Guid entityId)
        {
            var groupName = $"{entityType.ToLower()}_{entityId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveEntityGroup(string entityType, Guid entityId)
        {
            var groupName = $"{entityType.ToLower()}_{entityId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task JoinStaffGroup(Guid idStaff)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"staff_{idStaff}");
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