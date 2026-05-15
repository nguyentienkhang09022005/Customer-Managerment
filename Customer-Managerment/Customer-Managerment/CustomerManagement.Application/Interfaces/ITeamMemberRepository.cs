using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ITeamMemberRepository
    {
        Task<TeamMember?> GetByIdAsync(Guid id);
        Task<List<TeamMember>> GetByEntityAsync(string entityType, Guid entityId);
        Task<List<TeamMember>> GetByStaffAsync(Guid idStaff);
        Task<TeamMember?> GetTeamMemberAsync(string entityType, Guid entityId, Guid idStaff);
        Task<bool> HasOwnerAsync(string entityType, Guid entityId);
        Task<TeamMember> AddAsync(TeamMember teamMember);
        Task<TeamMember> UpdateAsync(TeamMember teamMember);
        Task<bool> RemoveAsync(Guid id);
        Task<int> CountByEntityAsync(string entityType, Guid entityId);
    }
}