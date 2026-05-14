using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface INoteMentionRepository
    {
        Task<NoteMention> AddMentionAsync(NoteMention mention);
        Task<IQueryable<NoteMention>> GetMentionsByNoteAsync(Guid idNote);
        Task<IQueryable<NoteMention>> GetMentionsByStaffAsync(Guid idStaff);
        Task DeleteMentionsByNoteAsync(Guid idNote);
    }
}