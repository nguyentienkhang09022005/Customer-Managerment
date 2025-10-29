using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ILeadRepository
    {
        Task<LeadDomain> AddLeadAsync(LeadDomain leadDomain);

        Task<LeadDomain?> GetLeadByIdAsync(Guid idLead);

        Task<List<LeadDomain>> GetListLeadAsync();

        Task<LeadDomain?> UpdateLeadAsync(LeadDomain leadDomain);

        Task DeleteLeadAsync(Guid idLead);

        Task<bool> CheckLeadExistsAsync(Guid idLead);

        Task<bool> checkPersonByEmailAsync(string email);
    }
}
