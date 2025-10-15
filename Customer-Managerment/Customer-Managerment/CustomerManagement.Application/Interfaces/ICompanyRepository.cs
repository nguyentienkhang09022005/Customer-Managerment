using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetListCompanyAsync();

    }
}
