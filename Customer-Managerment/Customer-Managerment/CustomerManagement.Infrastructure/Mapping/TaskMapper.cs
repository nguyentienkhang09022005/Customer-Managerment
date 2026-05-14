using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class TaskMapper : Profile
    {
        public TaskMapper()
        {
            // TaskEntity -> TaskResponse
            CreateMap<TaskEntity, TaskResponse>()
                .ForMember(dest => dest.IdTask, opt => opt.MapFrom(src => src.IdTask))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => TaskPriorityConstant.ToString(src.Priority)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TaskStatusConstant.ToString(src.Status)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
                .ForMember(dest => dest.IdStaffAssigned, opt => opt.MapFrom(src => src.IdStaffAssigned))
                .ForMember(dest => dest.LinkedEntityType, opt => opt.MapFrom(src => src.LinkedEntityType))
                .ForMember(dest => dest.LinkedEntityId, opt => opt.MapFrom(src => src.LinkedEntityId))
                .ForMember(dest => dest.StaffAssigned, opt => opt.MapFrom(src => src.IdStaffAssignedNavigation));

            // TaskCreationRequest -> TaskEntity
            CreateMap<TaskCreationRequest, TaskEntity>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.IdStaffAssigned, opt => opt.MapFrom(src => src.IdStaffAssigned))
                .ForMember(dest => dest.LinkedEntityType, opt => opt.MapFrom(src => src.LinkedEntityType))
                .ForMember(dest => dest.LinkedEntityId, opt => opt.MapFrom(src => src.LinkedEntityId))
                .ForMember(dest => dest.IdTask, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IdStaffAssignedNavigation, opt => opt.Ignore());

            // TaskUpdateRequest -> TaskEntity
            CreateMap<TaskUpdateRequest, TaskEntity>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.IdStaffAssigned, opt => opt.MapFrom(src => src.IdStaffAssigned))
                .ForMember(dest => dest.LinkedEntityType, opt => opt.MapFrom(src => src.LinkedEntityType))
                .ForMember(dest => dest.LinkedEntityId, opt => opt.MapFrom(src => src.LinkedEntityId));
        }
    }
}