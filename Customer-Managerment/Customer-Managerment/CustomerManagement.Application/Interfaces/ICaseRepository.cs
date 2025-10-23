using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ICaseRepository
    {
        Task<List<Case>> GetListCasesAsync(Guid idUser);

        Task<Case> AddCaseAsync(CaseDomain caseDomain);

        Task<Case> UpdateCaseAsync(CaseDomain caseDomain, Case caseEntity);

        Task<Case> GetCaseByIdAsync(Guid idCase);

        Task DeleteCaseAsync(Guid idCase);

        Task<Case> GetExistingCaseAsync(Guid idCase);
    }
}
