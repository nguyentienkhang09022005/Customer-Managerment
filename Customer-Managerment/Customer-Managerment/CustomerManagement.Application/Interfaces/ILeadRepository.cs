using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ILeadRepository
    {
        Task<Person> AddLeadAsync(Person lead);
        Task<Person?> GetLeadByIdAsync(Guid idLead);
        IQueryable<Person> GetListLead();
        Task<List<Person>> GetListLeadAsync();
        IQueryable<Person> GetLeadById(Guid idLead);
        Task<Person?> UpdateLeadAsync(Person lead);
        Task<bool> CheckLeadExistsAsync(Guid idLead);
        Task<bool> CheckPersonByEmailAsync(string email);
        Task<int> GetTotalLeadsAsync();
        Task<bool> SoftDeleteLeadAsync(Guid idLead);
        Task<bool> RestoreLeadAsync(Guid idLead);
    }
}