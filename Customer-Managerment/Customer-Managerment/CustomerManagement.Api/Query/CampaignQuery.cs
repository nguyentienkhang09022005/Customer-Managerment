using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.Campaigns;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class CampaignQuery
    {
        private readonly CampaignHandler _handler;

        public CampaignQuery(CampaignHandler handler)
        {
            _handler = handler;
        }

        //[Authorize]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public async Task<List<CampaignResponse>> GetCampaignsAsync(Guid idUser)
        {
            return await _handler.GetListCampaignsAsync(idUser);
        }

        //[Authorize]
        public async Task<CampaignResponse> GetInfCampaignAsync(Guid idCampaign)
        {
            return await _handler.GetInfCampaignAsync(idCampaign);

        }
    }
}
