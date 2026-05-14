using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class DealMapper : Profile
    {
        public DealMapper()
        {
            // Deal -> DealResponse
            CreateMap<Deal, DealResponse>()
                .ForMember(dest => dest.IdDeal, opt => opt.MapFrom(src => src.IdDeal))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.IdCustomerNavigation))
                .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src.IdStaffNavigation));

            // DealCreationRequest -> Deal
            CreateMap<DealCreationRequest, Deal>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.IdStaff, opt => opt.MapFrom(src => src.IdStaff))
                .ForMember(dest => dest.IdCustomer, opt => opt.MapFrom(src => src.IdCustomer))
                .ForMember(dest => dest.IdDeal, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IdStaffNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdCustomerNavigation, opt => opt.Ignore());

            // DealUpdateRequest -> Deal
            CreateMap<DealUpdateRequest, Deal>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            // Deal -> ListSuccessfullDealResponse
            CreateMap<Deal, ListSuccessfullDealResponse>()
                .ForMember(dest => dest.IdDeal, opt => opt.MapFrom(src => src.IdDeal))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // Deal -> ListFailedDealResponse
            CreateMap<Deal, ListFailedDealResponse>()
                .ForMember(dest => dest.IdDeal, opt => opt.MapFrom(src => src.IdDeal))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
}