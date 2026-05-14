using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class DealMutation
    {
        private readonly DealHandler _dealHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DealMutation(DealHandler dealHandler, IHttpContextAccessor httpContextAccessor)
        {
            _dealHandler = dealHandler;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DealResponse> CreateDealAsync(DealCreationRequest dealCreationRequest)
        {
            return await _dealHandler.CreateDealAsync(dealCreationRequest);
        }

        public async Task<string> DeleteDealAsync(Guid idDeal)
        {
            return await _dealHandler.DeleteDealAsync(idDeal);
        }

        public async Task<DealResponse> UpdateDealAsync(DealUpdateRequest dealUpdateRequest, Guid idDeal)
        {
            return await _dealHandler.UpdateDealAsync(dealUpdateRequest, idDeal);
        }

        private string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        }
    }
}
