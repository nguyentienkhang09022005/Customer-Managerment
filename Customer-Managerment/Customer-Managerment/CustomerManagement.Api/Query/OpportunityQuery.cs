using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class OpportunityQuery
    {
        private readonly OpportunityHandler _handler;

        public OpportunityQuery(OpportunityHandler handler)
        {
            _handler = handler;
        }

        [Authorize]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public async Task<List<OpportunityResponse>> GetOpportunitiesAsync(Guid idUser)
        {
            return await _handler.GetListOpportunitiesAsync(idUser);
        }

        [Authorize]
        public async Task<OpportunityResponse> GetInfOpportunityAsync(Guid idOpportunity)
        {
            return await _handler.GetInfOpportunityAsync(idOpportunity);
        }
    }
}
