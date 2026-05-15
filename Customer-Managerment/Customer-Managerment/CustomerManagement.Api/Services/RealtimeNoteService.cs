using Microsoft.AspNetCore.SignalR;
using Customer_Managerment.CustomerManagement.Api.Hubs;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;

namespace Customer_Managerment.CustomerManagement.Api.Services
{
    public interface IRealtimeNoteService
    {
        Task SendNoteToEntityAsync(string entityType, Guid entityId, NoteResponse note);
        Task SendNoteToStaffAsync(Guid idStaff, NoteResponse note);
        Task BroadcastNoteUpdateAsync(NoteResponse note);
    }

    public class RealtimeNoteService : IRealtimeNoteService
    {
        private readonly IHubContext<NoteHub> _hubContext;

        public RealtimeNoteService(IHubContext<NoteHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNoteToEntityAsync(string entityType, Guid entityId, NoteResponse note)
        {
            var groupName = $"{entityType.ToLower()}_{entityId}";
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNote", note);
        }

        public async Task SendNoteToStaffAsync(Guid idStaff, NoteResponse note)
        {
            await _hubContext.Clients.Group($"staff_{idStaff}").SendAsync("ReceiveNote", note);
        }

        public async Task BroadcastNoteUpdateAsync(NoteResponse note)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNote", note);
        }
    }
}