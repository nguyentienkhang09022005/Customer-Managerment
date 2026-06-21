using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class AuditLogMapper : Profile
    {
        public AuditLogMapper()
        {
            CreateMap<AuditLog, AuditLogResponse>()
                .ForMember(dest => dest.IdLog, opt => opt.MapFrom(src => src.IdLog))
                .ForMember(dest => dest.Action, opt => opt.MapFrom(src => src.Action))
                .ForMember(dest => dest.EntityType, opt => opt.MapFrom(src => src.EntityType))
                .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.EntityId))
                .ForMember(dest => dest.OldValues, opt => opt.MapFrom(src => src.OldValues))
                .ForMember(dest => dest.NewValues, opt => opt.MapFrom(src => src.NewValues))
                .ForMember(dest => dest.IdStaff, opt => opt.MapFrom(src => src.IdStaff))
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.StaffName))
                .ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.IpAddress))
                .ForMember(dest => dest.UserAgent, opt => opt.MapFrom(src => src.UserAgent))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
        }
    }
}
