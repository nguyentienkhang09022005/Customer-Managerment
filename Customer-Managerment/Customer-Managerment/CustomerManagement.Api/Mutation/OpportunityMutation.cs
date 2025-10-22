using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.Opportunity;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class OpportunityMutation
    {
        private readonly OpportunityHandler _handler;

        public OpportunityMutation(OpportunityHandler handler)
        {
            _handler = handler;
        }

        //[Authorize]
        public async Task<OpportunityResponse> CreateOpportunityAsync(OpportunityCreationRequest request)
        {
            return await _handler.CreateOpportunityAsync(request);
        }

        //[Authorize]
        public async Task<OpportunityResponse> UpdateOpportunityAsync(OpportunityUpdateRequest request, Guid idOpportunity)
        {
            return await _handler.UpdateOpportunityAsync(request, idOpportunity);
        }

        //[Authorize]
        public async Task<string> DeleteOpportunityAsync(Guid idOpportunity)
        {
            return await _handler.DeleteOpportunityAsync(idOpportunity);
        }
    }
}
