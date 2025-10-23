using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ICampaignRepository
    {
        Task<List<Campaign>> GetListCampaignsAsync(Guid idUser);

        Task<Campaign> AddCampaignAsync(CampaignDomain campaignDomain);

        Task<Campaign> GetCampaignByIdAsync(Guid idCampaign);

        Task<Campaign> UpdateCampaignAsync(CampaignDomain campaignDomain, Campaign campaign);

        Task DeleteCampaignAsync(Guid idCampaign);

        Task<Campaign> GetExistingCampaignAsync(Guid idCampaign);
    }
}
