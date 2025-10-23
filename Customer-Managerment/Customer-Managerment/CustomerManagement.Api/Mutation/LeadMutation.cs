using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.Leads;
using HotChocolate.Authorization;

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

        [Authorize]
        public async Task<LeadResponse> CreateLeadAsync(LeadCreationRequest request)
        {
            return await _leadHandler.CreateLeadAsync(request);
        }

        [Authorize]
        public async Task<LeadResponse> UpdateLeadAsync(Guid idLead, LeadUpdateRequest request)
        {
            return await _leadHandler.UpdateLeadAsync(idLead, request);
        }

        [Authorize]
        public async Task<string> DeleteLeadAsync(Guid idLead)
        {
            return await _leadHandler.DeleteLeadAsync(idLead);

        }
    }
}
