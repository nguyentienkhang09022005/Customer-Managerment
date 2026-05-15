using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Api.Services;

namespace Customer_Managerment.CustomerManagement.Api.Services
{
    public class RealtimeNoteSenderAdapter : IRealtimeNoteSender
    {
        private readonly IRealtimeNoteService _realtimeService;

        public RealtimeNoteSenderAdapter(IRealtimeNoteService realtimeService)
        {
            _realtimeService = realtimeService;
        }

        public async Task SendNoteToEntityAsync(string entityType, Guid entityId, NoteResponse note)
        {
            await _realtimeService.SendNoteToEntityAsync(entityType, entityId, note);
        }

        public async Task SendNoteToStaffAsync(Guid idStaff, NoteResponse note)
        {
            await _realtimeService.SendNoteToStaffAsync(idStaff, note);
        }
    }
}