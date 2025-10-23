using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.Leads;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class LeadQuery
    {
        private readonly LeadHandler _leadHandler;

        public LeadQuery(LeadHandler leadHandler)
        {
            _leadHandler = leadHandler;
        }

        [Authorize]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public async Task<List<LeadResponse>> GetAllLeadsAsync()
        {
            return await _leadHandler.GetAllLeadsAsync();
        }

        [Authorize]
        public async Task<LeadResponse> GetLeadByIdAsync(Guid idLead)
        {
            return await _leadHandler.GetLeadByIdAsync(idLead);
        }
    }
}
