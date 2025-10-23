using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class CampaignMapper : Profile
    {
        public CampaignMapper()
        {
            CreateMap<Campaign, CampaignDomain>();

            CreateMap<CampaignDomain, Campaign>()
                .ForMember(dest => dest.IdCampaign, opt => opt.Ignore());

            CreateMap<Campaign, CampaignResponse>();

            CreateMap<CampaignCreationRequest, CampaignDomain>();

            CreateMap<CampaignUpdateRequest, CampaignDomain>();
        }
    }
}
