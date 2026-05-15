using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class LeadMutation
    {
        private readonly LeadHandler _leadHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LeadMutation(LeadHandler leadHandler, IHttpContextAccessor httpContextAccessor)
        {
            _leadHandler = leadHandler;
            _httpContextAccessor = httpContextAccessor;
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

        // File upload moved to /api/fileupload/lead

        private string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        }
    }
}
