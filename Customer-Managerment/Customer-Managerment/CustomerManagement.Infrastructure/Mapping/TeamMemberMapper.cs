using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class TeamMemberMapper : Profile
    {
        public TeamMemberMapper()
        {
            CreateMap<TeamMember, TeamMemberResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.EntityType, opt => opt.MapFrom(src => src.EntityType))
                .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.EntityId))
                .ForMember(dest => dest.IdStaff, opt => opt.MapFrom(src => src.IdStaff))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => TeamRoleConstant.ToString(src.Role)))
                .ForMember(dest => dest.AssignedAt, opt => opt.MapFrom(src => src.AssignedAt))
                .ForMember(dest => dest.AssignedBy, opt => opt.MapFrom(src => src.AssignedBy))
                .ForMember(dest => dest.CanEdit, opt => opt.MapFrom(src => src.CanEdit))
                .ForMember(dest => dest.CanDelete, opt => opt.MapFrom(src => src.CanDelete))
                .ForMember(dest => dest.Staff, opt => opt.Ignore());
        }
    }
}