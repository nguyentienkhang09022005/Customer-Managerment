using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.Campaigns;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class CampaignMutation
    {
        private readonly CampaignHandler _handler;

        public CampaignMutation(CampaignHandler handler)
        {
            _handler = handler;
        }

        [Authorize]
        public async Task<CampaignResponse> CreateCampaignAsync(CampaignCreationRequest request)
        {
            return await _handler.CreateCampaignAsync(request);
        }

        [Authorize]
        public async Task<CampaignResponse> UpdateCampaignAsync(CampaignUpdateRequest request, Guid idCampaign)
        {
            return await _handler.UpdateCampaignAsync(request, idCampaign);
        }

        [Authorize]
        public async Task<string> DeleteCampaignAsync(Guid idCampaign)
        {
            return await _handler.DeleteCampaignAsync(idCampaign);
        }
    }
}
