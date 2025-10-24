using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class StaffMapper : Profile
    {
        public StaffMapper()
        {
            CreateMap<Staff, StaffDomain>();

            CreateMap<StaffDomain, Staff>()
                .ForMember(dest => dest.IdStaff, opt => opt.Ignore());

            CreateMap<StaffDomain, StaffResponse>();

            CreateMap<Staff, StaffResponse>();
        }
    }
}
