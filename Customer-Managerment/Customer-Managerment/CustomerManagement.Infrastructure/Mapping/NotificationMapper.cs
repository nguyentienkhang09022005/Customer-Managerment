using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class NotificationMapper : Profile
    {
        public NotificationMapper()
        {
            // Notification -> NotificationResponse
            CreateMap<Notification, NotificationResponse>()
                .ForMember(dest => dest.IdNotification, opt => opt.MapFrom(src => src.IdNotification))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
                .ForMember(dest => dest.IsPinned, opt => opt.MapFrom(src => src.IsPinned))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.IdStaff, opt => opt.MapFrom(src => src.IdStaff))
                .ForMember(dest => dest.RelatedEntityType, opt => opt.MapFrom(src => src.RelatedEntityType))
                .ForMember(dest => dest.RelatedEntityId, opt => opt.MapFrom(src => src.RelatedEntityId))
                .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src.IdStaffNavigation));
        }
    }
}