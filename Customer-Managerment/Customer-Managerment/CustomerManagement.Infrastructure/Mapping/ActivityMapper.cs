using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class ActivityMapper : Profile
    {
        public ActivityMapper()
        {
            CreateMap<Activity, ActivityDomain>();

            CreateMap<ActivityDomain, Activity>()
                .ForMember(dest => dest.IdActivity, opt => opt.Ignore());

            CreateMap<ActivityDomain, ActivityResponse>();

            CreateMap<Activity, ActivityResponse>()
                .ForMember(dest => dest.EmployeeName,
                           opt => opt.MapFrom(src => src.IdUserNavigation != null ? src.IdUserNavigation.Fullname : null))
                .ForMember(dest => dest.CustomerName,
                           opt => opt.MapFrom(src => src.IdCustomerNavigation != null ? src.IdCustomerNavigation.Fullname : null));

            CreateMap<ActivityCreationRequest, ActivityDomain>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));

            CreateMap<ActivityUpdateRequest, ActivityDomain>();
        }
    }
}
