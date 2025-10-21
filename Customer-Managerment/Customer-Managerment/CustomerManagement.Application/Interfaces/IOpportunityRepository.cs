using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IOpportunityRepository
    {
        Task<List<Opportunity>> GetListOpportunitiesAsync(Guid idUser);

        Task<Opportunity> AddOpportunityAsync(OpportunityDomain opportunityDomain);

        Task<Opportunity> GetOpportunityByIdAsync(Guid idOpportunity);

        Task<Opportunity> UpdateOpportunityAsync(OpportunityDomain opportunityDomain, Opportunity opportunity);

        Task DeleteOpportunityAsync(Guid idOpportunity);

        Task<Opportunity> GetExistingOpportunityAsync(Guid idOpportunity);

        Task<bool> CheckOpportunityExistsAsync(Guid idOpportunity);
    }
}
