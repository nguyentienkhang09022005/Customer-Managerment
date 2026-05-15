using Customer_Managerment.CustomerManagement.Application.DTOs.Response;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IRealtimeNoteSender
    {
        Task SendNoteToEntityAsync(string entityType, Guid entityId, NoteResponse note);
        Task SendNoteToStaffAsync(Guid idStaff, NoteResponse note);
    }
}