using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class DealMapper : Profile
    {
        public DealMapper()
        {
            CreateMap<Deal, DealDomain>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.IdCustomerNavigation))
                .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src.IdStaffNavigation));

            CreateMap<DealDomain, Deal>()
                .ForMember(dest => dest.IdDeal, opt => opt.Ignore());

            CreateMap<DealDomain, DealResponse>()
                .ForMember(dest => dest.infCustomerResponse, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.infStaffResponse, opt => opt.MapFrom(src => src.Staff));


            CreateMap<DealCreationRequest, DealDomain>();

            CreateMap<DealUpdateRequest, DealDomain>();
        }
    }
}
