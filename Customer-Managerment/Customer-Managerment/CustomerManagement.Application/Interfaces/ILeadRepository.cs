using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ILeadRepository
    {
        Task<List<Lead>> GetAllLeadsAsync();

        Task<List<Lead>> GetLeadsByCampaignAsync(Guid idCampaign);

        Task<Lead> GetLeadByIdAsync(Guid idLead);

        Task<Lead> AddLeadAsync(LeadDomain leadDomain);

        Task<Lead> UpdateLeadAsync(LeadDomain leadDomain, Lead leadEntity);

        Task DeleteLeadAsync(Guid idLead);

        Task<bool> CheckLeadExistsAsync(Guid idLead);

        Task<Lead> GetExistingLeadAsync(Guid idLead);
    }
}
