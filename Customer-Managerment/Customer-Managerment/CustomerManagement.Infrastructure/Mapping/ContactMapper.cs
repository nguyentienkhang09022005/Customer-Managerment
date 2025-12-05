using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class ContactMapper : Profile
    {
        public ContactMapper()
        {
            CreateMap<Contact, ContactDomain>()
                .ForMember(dest => dest.Lead, opt => opt.MapFrom(src => src.IdLeadNavigation))
                .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src.IdStaffNavigation));

            CreateMap<ContactDomain, Contact>()
                .ForMember(dest => dest.IdContact, opt => opt.Ignore());

            CreateMap<ContactDomain, ContactResponse>()
                .ForMember(dest => dest.infLeadResponse, opt => opt.MapFrom(src => src.Lead))
                .ForMember(dest => dest.infStaffResponse, opt => opt.MapFrom(src => src.Staff));

            CreateMap<ContactCreationRequest, ContactDomain>();

            CreateMap<ContactUpdateRequest, ContactDomain>();
        }
    }
}
