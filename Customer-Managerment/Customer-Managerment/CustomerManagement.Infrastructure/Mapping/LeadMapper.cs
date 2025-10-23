using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class LeadMapper : Profile
    {
        public LeadMapper()
        {
            CreateMap<Lead, LeadDomain>();

            CreateMap<LeadDomain, Lead>()
                .ForMember(dest => dest.IdLead, opt => opt.Ignore());

            CreateMap<LeadDomain, LeadResponse>();

            CreateMap<Lead, LeadResponse>();

            CreateMap<LeadCreationRequest, LeadDomain>();

            CreateMap<LeadUpdateRequest, LeadDomain>();
        }
    }
}
