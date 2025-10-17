using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.Company;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class CompanyMutation
    {
        private readonly CompanyHandler _companyHandler;
        private readonly ILogger<CompanyMutation> _logger;

        public CompanyMutation(CompanyHandler companyHandler, ILogger<CompanyMutation> logger)
        {
            _companyHandler = companyHandler;
            _logger = logger;
        }

        // Company Mutation
        [Authorize]
        public async Task<CompanyResponse> CreateCompanyAsync(CompanyRequest companyRequest)
        {
            return await _companyHandler.CreateCompanyAsync(companyRequest);
        }

        [Authorize]
        public async Task<CompanyResponse> UpdateCompanyAsync(CompanyRequest companyRequest, Guid IdCompany)
        {
            return await _companyHandler.UpdateCompanyAsync(companyRequest, IdCompany);
        }

        [Authorize]
        public async Task<string> DeleteCompanyAsync(Guid idCompany)
        {
            return await _companyHandler.DeleteCompanyAsync(idCompany);
        }
    }
}
