using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class LeadMutation
    {
        private readonly LeadHandler _leadHandler;

        public LeadMutation(LeadHandler leadHandler)
        {
            _leadHandler = leadHandler;
        }

        public async Task<LeadResponse> CreateLeadAsync(LeadCreationRequest leadCreationRequest)
        {
            return await _leadHandler.CreateLeadAsync(leadCreationRequest);
        }

        public async Task<string> DeleteLeadAsync(Guid idLead)
        {
            return await _leadHandler.DeleteLeadAsync(idLead);
        }

        public async Task<LeadResponse> UpdateLeadAsync(LeadUpdateRequest leadUpdateRequest, Guid idLead)
        {
            return await _leadHandler.UpdateLeadAsync(leadUpdateRequest, idLead);
        }

        public async Task<string> ImportLeadExcelAsync(IFile file)
        {
            return await _leadHandler.ImportLeadExcelAsync(file);
        }
    }
}
