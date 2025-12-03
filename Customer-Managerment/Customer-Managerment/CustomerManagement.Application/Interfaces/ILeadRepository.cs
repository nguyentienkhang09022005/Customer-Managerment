using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ILeadRepository
    {
        Task<LeadDomain> AddLeadAsync(LeadDomain leadDomain);

        Task<LeadDomain?> GetLeadByIdAsync(Guid idLead);

        IQueryable<LeadDomain> GetListLead();

        Task<List<LeadDomain>> GetListLeadAsync();

        IQueryable<LeadDomain> GetLeadById(Guid idLead);

        Task<LeadDomain?> UpdateLeadAsync(LeadDomain leadDomain);

        Task DeleteLeadAsync(Guid idLead);

        Task<bool> CheckLeadExistsAsync(Guid idLead);

        Task<bool> checkPersonByEmailAsync(string email);

        Task<int> getTotalLeadsAsync();
    }
}
