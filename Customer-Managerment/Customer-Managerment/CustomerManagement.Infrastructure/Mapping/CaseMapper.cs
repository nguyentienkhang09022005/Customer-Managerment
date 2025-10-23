using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class CaseMapper : Profile
    {
        public CaseMapper()
        {
            CreateMap<Case, CaseDomain>();

            CreateMap<CaseDomain, Case>()
                .ForMember(dest => dest.IdCase, opt => opt.Ignore());

            CreateMap<Case, CaseResponse>()
                .ForMember(dest => dest.EmployeeName,
                           opt => opt.MapFrom(src => src.IdUserNavigation != null ? src.IdUserNavigation.Fullname : null))
                .ForMember(dest => dest.CustomerName,
                           opt => opt.MapFrom(src => src.IdCustomerNavigation != null ? src.IdCustomerNavigation.Fullname : null));

            CreateMap<CaseCreationRequest, CaseDomain>();

            CreateMap<CaseUpdateRequest, CaseDomain>();
        }
    }
}
