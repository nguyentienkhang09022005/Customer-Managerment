using Customer_Managerment.CustomerManagement.Api.Input.Type;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    [Authorize]
    public class TaskMutation
    {
        private readonly TaskHandler _taskHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TaskMutation(TaskHandler taskHandler, IHttpContextAccessor httpContextAccessor)
        {
            _taskHandler = taskHandler;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TaskResponse> CreateTaskAsync(TaskInput input)
        {
            DateTime? dueDate = null;
            if (!string.IsNullOrEmpty(input.DueDate) && DateTime.TryParse(input.DueDate, out var parsed))
            {
                dueDate = parsed;
            }

            var request = new TaskCreationRequest
            {
                Title = input.Title,
                Description = input.Description,
                DueDate = dueDate,
                Priority = (int)input.Priority,
                Status = (int)input.Status,
                IdStaffAssigned = input.IdStaffAssigned,
                LinkedEntityType = input.LinkedEntityType,
                LinkedEntityId = input.LinkedEntityId
            };

            return await _taskHandler.CreateTaskAsync(request);
        }

        public async Task<TaskResponse> UpdateTaskAsync(TaskUpdateInput input, Guid idTask)
        {
            DateTime? dueDate = null;
            if (!string.IsNullOrEmpty(input.DueDate) && DateTime.TryParse(input.DueDate, out var parsed))
            {
                dueDate = parsed;
            }

            var request = new TaskUpdateRequest
            {
                Title = input.Title,
                Description = input.Description,
                DueDate = dueDate,
                Priority = (int?)input.Priority,
                Status = (int?)input.Status,
                IdStaffAssigned = input.IdStaffAssigned,
                LinkedEntityType = input.LinkedEntityType,
                LinkedEntityId = input.LinkedEntityId
            };

            return await _taskHandler.UpdateTaskAsync(request, idTask);
        }

        public async Task<bool> DeleteTaskAsync(Guid idTask)
        {
            await _taskHandler.DeleteTaskAsync(idTask);
            return true;
        }

        public async Task<TaskResponse> RestoreTaskAsync(Guid idTask)
        {
            return await _taskHandler.RestoreTaskAsync(idTask);
        }

        public async Task<TaskResponse> AssignTaskAsync(Guid idTask, Guid idStaff)
        {
            var assignedBy = GetCurrentUserId();
            return await _taskHandler.AssignTaskAsync(idTask, idStaff, assignedBy);
        }

        public async Task<TaskResponse> UpdateTaskStatusAsync(Guid idTask, int status)
        {
            var updatedBy = GetCurrentUserId();
            return await _taskHandler.UpdateTaskStatusAsync(idTask, status, updatedBy);
        }

        private string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst("sub")?.Value ?? "system";
        }
    }
}