using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Application.UseCases;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class TaskQuery
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public TaskQuery(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<TaskResponse> GetTasks()
        {
            var tasks = _taskRepository.GetListTaskAsync().Result;
            return tasks.ProjectTo<TaskResponse>(_mapper.ConfigurationProvider);
        }

        public TaskResponse? GetTaskById(Guid idTask)
        {
            var task = _taskRepository.GetTaskByIdAsync(idTask).Result;
            return task == null ? null : _mapper.Map<TaskResponse>(task);
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<TaskResponse> GetTasksByStaff(Guid idStaff)
        {
            var tasks = _taskRepository.GetTasksByStaffAsync(idStaff).Result;
            return tasks.ProjectTo<TaskResponse>(_mapper.ConfigurationProvider);
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<TaskResponse> GetTasksByStatus(int status)
        {
            var tasks = _taskRepository.GetTasksByStatusAsync(status).Result;
            return tasks.ProjectTo<TaskResponse>(_mapper.ConfigurationProvider);
        }
    }
}