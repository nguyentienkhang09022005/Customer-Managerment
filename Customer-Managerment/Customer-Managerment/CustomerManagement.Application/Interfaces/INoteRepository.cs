using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface INoteRepository
    {
        Task<Note?> GetNoteByIdAsync(Guid idNote);
        Task<IQueryable<Note>> GetNotesByEntityAsync(string entityType, Guid entityId);
        Task<IQueryable<Note>> GetPinnedNotesAsync(string entityType, Guid entityId);
        Task<Note> AddNoteAsync(Note note);
        Task<Note> UpdateNoteAsync(Note note);
        Task<bool> SoftDeleteNoteAsync(Guid idNote);
    }
}