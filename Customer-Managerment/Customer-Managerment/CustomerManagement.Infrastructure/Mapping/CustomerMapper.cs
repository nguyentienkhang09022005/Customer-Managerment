using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class CustomerMapper : Profile
    {
        public CustomerMapper()
        {
            CreateMap<Customer, CustomerDomain>()
                .ForMember(dest => dest.personDomain, opt => opt.MapFrom(src => src.IdCustomerNavigation))
                .ReverseMap()
                .ForMember(dest => dest.IdCustomerNavigation, opt => opt.MapFrom(src => src.personDomain));

            CreateMap<CustomerCreationRequest, CustomerDomain>()
                .ForMember(dest => dest.personDomain, opt => opt.MapFrom(src => src.Person));

            CreateMap<CustomerUpdateRequest, CustomerDomain>()
                .ForMember(dest => dest.personDomain, opt => opt.MapFrom(src => src.Person));

            CreateMap<CustomerDomain, CustomerResponse>()
                .ForMember(dest => dest.personResponse, opt => opt.MapFrom(src => src.personDomain));
        }
    }
}
