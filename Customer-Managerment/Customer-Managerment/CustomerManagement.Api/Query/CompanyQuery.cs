using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.Company;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class CompanyQuery
    {
        private readonly CompanyHandler _companyHandler;

        public CompanyQuery(CompanyHandler companyHandler)
        {
            _companyHandler = companyHandler;
        }

        // Company Query
        [UseProjection]   
        [UseFiltering]
        [UseSorting]
        public async Task<List<CompanyResponse>> GetCompaniesAsync()
        {
            return await _companyHandler.GetListCompanyAsync();
        }

        public async Task<CompanyResponse> GetInfCompanyAsync(Guid idCompany)
        {
            return await _companyHandler.GetInfCompanyAsync(idCompany);
        }
    }
}
