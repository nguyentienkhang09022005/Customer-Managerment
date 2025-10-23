using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class TasksMutation
    {
        private readonly TasksHandler _tasksHandler;

        public TasksMutation(TasksHandler tasksHandler)
        {
            _tasksHandler = tasksHandler;
        }

        // Task Mutation
        [Authorize]
        public async Task<TasksResponse> CreateTaskAsync(TasksCreationRequest tasksCreationRequest)
        {
            return await _tasksHandler.CreateTaskAsync(tasksCreationRequest);
        }

        [Authorize]
        public async Task<TasksResponse> UpdateTaskAsync(TaskUpdateRequest taskUpdateRequest, Guid idTask)
        {
            return await _tasksHandler.UpdateTaskAsync(taskUpdateRequest, idTask);
        }

        [Authorize]
        public async Task<string> DeleteTaskAsync(Guid idTask)
        {
            return await _tasksHandler.DeleteTaskAsync(idTask);
        }
    }
}
