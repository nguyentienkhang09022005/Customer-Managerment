using Customer_Managerment.CustomerManagement.Application.DTOs;
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

        [UseProjection]   // Tự động chọn field tương ứng query
        [UseFiltering]
        [UseSorting]
        public async Task<List<CompanyDTO>> GetCompaniesAsync()
        {
            return await _companyHandler.GetListCompanyAsync();
        }
    }
}
