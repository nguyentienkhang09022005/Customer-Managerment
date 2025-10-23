using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class TasksQuery
    {
        private TasksHandler _tasksHandler;

        public TasksQuery(TasksHandler tasksHandler) 
        {
            _tasksHandler = tasksHandler;
        }

        // Task Query
        [Authorize]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public async Task<List<TasksResponse>> GetTasksAsync(Guid idUser)
        {
            return await _tasksHandler.GetListTasksAsync(idUser);
        }

        [Authorize]
        public async Task<TasksResponse> GetInfTaskAsync(Guid idTask)
        {
            return await _tasksHandler.GetInfTaskAsync(idTask);
        }
    }
}
