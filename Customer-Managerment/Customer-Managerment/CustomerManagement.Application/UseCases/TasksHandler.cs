using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using Customer_Managerment.CustomerManagement.Infrastructure.Repositories;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class TasksHandler
    {
        private readonly ITasksRepository _tasksRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public TasksHandler(ITasksRepository tasksRepository, IMapper mapper, IUserRepository userRepository)
        {
            _tasksRepository = tasksRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<List<TasksResponse>> GetListTasksAsync(Guid idUser)
        {
            var tasks = await _tasksRepository.GetListTasksAsync(idUser);
            return _mapper.Map<List<TasksResponse>>(tasks);
        }

        public async Task<TasksResponse> GetInfTaskAsync(Guid idTask)
        {
            var infTask = await _tasksRepository.GetTasksByIdAsync(idTask);
            return _mapper.Map<TasksResponse>(infTask);
        }

        public async Task<TasksResponse> CreateTaskAsync(TasksCreationRequest tasksCreationRequest)
        {
            bool userExists = await _userRepository.CheckUserExistsAsync(tasksCreationRequest.IdUser);
            if (!userExists)
                throw new DomainException("Người dùng không tồn tại, không thể tạo task.", 404);


            var taskDomain = _mapper.Map<TasksDomain>(tasksCreationRequest);
            var newTask = await _tasksRepository.AddTasksAsync(taskDomain);

            var companyResponse = _mapper.Map<TasksResponse>(newTask);
            return companyResponse;
        }

        public async Task<TasksResponse> UpdateTaskAsync(TaskUpdateRequest taskUpdateRequest, Guid idTask)
        {
            var taskEntity = await _tasksRepository.GetExistingTaskAsync(idTask);
            if (taskEntity == null){
                throw new DomainException("Không tìm thấy công việc cần đổi thông tin", 404);
            }
            var taskDomain = _mapper.Map<TasksDomain>(taskEntity);
            
            _mapper.Map(taskUpdateRequest, taskDomain);
            
            var updatedTask = await _tasksRepository.UpdateTaskAsync(taskDomain, taskEntity);
            
            return _mapper.Map<TasksResponse>(updatedTask);
        }

        public async Task<string> DeleteTaskAsync(Guid idTask)
        {
            await _tasksRepository.DeleteTaskAsync(idTask);
            return "Xóa công việc thành công!";
        }
    }
}
