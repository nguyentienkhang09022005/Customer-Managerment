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
            CreateMap<Person, PersonDomain>().ReverseMap();

            CreateMap<Lead, LeadDomain>()
                .ForMember(dest => dest.personDomain, opt => opt.MapFrom(src => src.IdLeadNavigation))
                .ReverseMap()
                .ForMember(dest => dest.IdLeadNavigation, opt => opt.MapFrom(src => src.personDomain));

            CreateMap<PersonCreationRequest, PersonDomain>();
            CreateMap<LeadCreationRequest, LeadDomain>()
                .ForMember(dest => dest.personDomain, opt => opt.MapFrom(src => src.Person));

            CreateMap<PersonUpdateRequest, PersonDomain>();
            CreateMap<LeadUpdateRequest, LeadDomain>()
                .ForMember(dest => dest.personDomain, opt => opt.MapFrom(src => src.Person));

            CreateMap<PersonDomain, PersonResponse>();
            CreateMap<LeadDomain, LeadResponse>();
        }
    }
}
