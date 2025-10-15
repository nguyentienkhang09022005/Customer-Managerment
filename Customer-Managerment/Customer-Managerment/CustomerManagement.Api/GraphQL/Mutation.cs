using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.Company;

namespace Customer_Managerment.CustomerManagement.Api.GraphQL
{
    public class Mutation
    {
        private readonly CompanyHandler _companyHandler;

        public Mutation(CompanyHandler companyHandler)
        {
            _companyHandler = companyHandler;
        }

        public async Task<CompanyResponse> CreateCompanyAsync(CompanyRequest companyRequest)
        {
            return await _companyHandler.CreateCompanyAsync(companyRequest);
        }
    }
}
