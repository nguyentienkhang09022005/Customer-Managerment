using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class OrderMapper : Profile
    {
        public OrderMapper()
        {
            CreateMap<Order, OrderDomain>();

            CreateMap<OrderDomain, Order>()
                .ForMember(dest => dest.IdOrder, opt => opt.Ignore());

            CreateMap<OrderDomain, OrderResponse>()
                .ForMember(dest => dest.Details, 
                           opt => opt.MapFrom(src => src.OrderDetailsDomain));

            CreateMap<OrderCreationRequest, OrderDomain>();

            CreateMap<OrderUpdateRequest, OrderDomain>();
        }
    }
}
