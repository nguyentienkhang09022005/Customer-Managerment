using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.Company;

namespace Customer_Managerment.CustomerManagement.Api.GraphQL
{
    public class Query
    {
        private readonly CompanyHandler _companyHandler;

        public Query(CompanyHandler companyHandler)
        {
            _companyHandler = companyHandler;
        }

        [UseProjection]   
        [UseFiltering]
        [UseSorting]
        public async Task<List<CompanyResponse>> GetCompaniesAsync()
        {
            return await _companyHandler.GetListCompanyAsync();
        }
    }
}
