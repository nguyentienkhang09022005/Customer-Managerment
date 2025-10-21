using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Mapping
{
    public class TasksMapper : Profile
    {
        public TasksMapper() 
        {
            // Tasks -> TasksDomain
            CreateMap<Tasks, TasksDomain>();

            // TasksDomain -> Tasks
            CreateMap<TasksDomain, Tasks>()
                .ForMember(dest => dest.IdTask, opt => opt.Ignore());

            // TasksDomain -> TasksResponse
            CreateMap<TasksDomain, TasksResponse>();

            // Tasks -> TasksResponse
            CreateMap<Tasks, TasksResponse>();

            // TasksCreationRequest -> TasksDomain
            CreateMap<TasksCreationRequest, TasksDomain>();

            // TaskUpdateRequest -> TasksDomain
            CreateMap<TaskUpdateRequest, TasksDomain>();
        }
    }
}
