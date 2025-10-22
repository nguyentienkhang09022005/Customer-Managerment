using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class OrderDetailMapper : Profile
    {
        public OrderDetailMapper()
        {
            CreateMap<OrderDetail, OrderDetailDomain>();

            CreateMap<OrderDetailDomain, OrderDetail>()
                .ForMember(dest => dest.IdOrderDetail, opt => opt.Ignore());
            
            CreateMap<OrderDetailDomain, OrderDetailResponse>()
                .ForMember(dest => dest.ProductName,
                           opt => opt.MapFrom(src => src.IdProductNavigation.ProductName)); 
        }
    }
}
