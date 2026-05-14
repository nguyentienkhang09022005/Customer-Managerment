using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class ContactMapper : Profile
    {
        public ContactMapper()
        {
            // Contact -> ContactResponse
            CreateMap<Contact, ContactResponse>()
                .ForMember(dest => dest.IdContact, opt => opt.MapFrom(src => src.IdContact))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.Lead, opt => opt.MapFrom(src => src.IdLeadNavigation))
                .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src.IdStaffNavigation));

            // ContactCreationRequest -> Contact
            CreateMap<ContactCreationRequest, Contact>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.IdStaff, opt => opt.MapFrom(src => src.IdStaff))
                .ForMember(dest => dest.IdLead, opt => opt.MapFrom(src => src.IdLead))
                .ForMember(dest => dest.IdContact, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IdStaffNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdLeadNavigation, opt => opt.Ignore());

            // ContactUpdateRequest -> Contact
            CreateMap<ContactUpdateRequest, Contact>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        }
    }
}