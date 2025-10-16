using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.Company;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class Query
    {
        private readonly CompanyHandler _companyHandler;

        public Query(CompanyHandler companyHandler)
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
    }
}
