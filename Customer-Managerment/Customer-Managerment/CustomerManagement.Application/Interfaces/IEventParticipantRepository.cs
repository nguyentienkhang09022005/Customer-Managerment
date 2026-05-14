using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IEventParticipantRepository
    {
        Task<EventParticipant?> GetByIdAsync(Guid id);
        Task<IQueryable<EventParticipant>> GetByEventAsync(Guid idEvent);
        Task<EventParticipant?> GetParticipantAsync(Guid idEvent, Guid idStaff);
        Task<EventParticipant> AddAsync(EventParticipant participant);
        Task<EventParticipant> UpdateAsync(EventParticipant participant);
        Task<bool> RemoveAsync(Guid id);
        Task<bool> RemoveByEventAsync(Guid idEvent);
    }
}