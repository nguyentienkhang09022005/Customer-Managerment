using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetListCompanyAsync();

        Task<Company> AddCompanyAsync(CompanyDomain companyDomain);

        Task<Company> GetCompanyByIdAsync(Guid idCompany);

        Task<Company> UpdateCompanyAsync(CompanyDomain companyDomain, Guid idCompany);

        Task DeleteCompanyAsync(Guid idCompany);
    }
}
