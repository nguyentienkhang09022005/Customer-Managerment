using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class TaskHandler
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IStaffRepository _staffRepository;
        // private readonly IElasticsearchService _elasticsearchService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public TaskHandler(
            ITaskRepository taskRepository,
            IStaffRepository staffRepository,
            INotificationRepository notificationRepository,
            // IElasticsearchService elasticsearchService,
            IMapper mapper)
        {
            _taskRepository = taskRepository;
            _staffRepository = staffRepository;
            _notificationRepository = notificationRepository;
            // _elasticsearchService = elasticsearchService;
            _mapper = mapper;
        }

        public async Task<TaskResponse> CreateTaskAsync(TaskCreationRequest request)
        {
            ValidateTaskCreation(request);

            var staff = await _staffRepository.GetStaffByIdAsync(request.IdStaffAssigned);
            if (staff == null)
            {
                throw new StaffNotFoundException();
            }

            var task = _mapper.Map<TaskEntity>(request);
            task.Status = TaskStatusConstant.TaskStatusPending.ToStatusInt();

            var createdTask = await _taskRepository.AddTaskAsync(task);
            var response = _mapper.Map<TaskResponse>(createdTask);
            response.StaffAssigned = _mapper.Map<StaffResponse>(staff);

            // Create notification for assigned staff
            var notification = new Notification
            {
                Title = "Bạn được giao công việc mới",
                Message = $"Bạn được giao công việc: {createdTask.Title}",
                Type = NotificationTypeConstant.NotificationTaskAssigned,
                IdStaff = request.IdStaffAssigned,
                RelatedEntityType = "Task",
                RelatedEntityId = createdTask.IdTask
            };
            await _notificationRepository.AddNotificationAsync(notification);

            // await _elasticsearchService.IndexAsync(response, "tasks");

            return response;
        }

        public async Task<string> DeleteTaskAsync(Guid idTask)
        {
            var result = await _taskRepository.SoftDeleteTaskAsync(idTask);
            if (!result)
            {
                throw new TaskNotFoundException();
            }

            // await _elasticsearchService.DeleteAsync<TaskResponse>(idTask.ToString(), "tasks");
            return "Xóa công việc thành công!";
        }

        public async Task<TaskResponse> RestoreTaskAsync(Guid idTask)
        {
            var result = await _taskRepository.RestoreTaskAsync(idTask);
            if (!result)
            {
                throw new TaskNotFoundException();
            }

            var task = await _taskRepository.GetTaskByIdAsync(idTask);
            var response = _mapper.Map<TaskResponse>(task);

            // await _elasticsearchService.IndexAsync(response, "tasks");
            return response;
        }

        public async Task<TaskResponse> UpdateTaskAsync(TaskUpdateRequest request, Guid idTask)
        {
            var existingTask = await _taskRepository.GetTaskByIdAsync(idTask);
            if (existingTask == null)
            {
                throw new TaskNotFoundException();
            }

            ValidateTaskUpdate(request);

            if (!string.IsNullOrEmpty(request.Title))
                existingTask.Title = request.Title;
            if (request.Description != null)
                existingTask.Description = request.Description;
            if (request.DueDate.HasValue)
                existingTask.DueDate = request.DueDate;
            if (request.Priority.HasValue)
                existingTask.Priority = request.Priority.Value;
            if (request.Status.HasValue)
                existingTask.Status = request.Status.Value;
            if (request.IdStaffAssigned.HasValue)
                existingTask.IdStaffAssigned = request.IdStaffAssigned.Value;
            if (!string.IsNullOrEmpty(request.LinkedEntityType))
                existingTask.LinkedEntityType = request.LinkedEntityType;
            if (request.LinkedEntityId.HasValue)
                existingTask.LinkedEntityId = request.LinkedEntityId;

            existingTask.UpdatedAt = DateTime.UtcNow;

            var updatedTask = await _taskRepository.UpdateTaskAsync(existingTask);
            var staff = await _staffRepository.GetStaffByIdAsync(updatedTask.IdStaffAssigned);
            var response = _mapper.Map<TaskResponse>(updatedTask);
            response.StaffAssigned = staff != null ? _mapper.Map<StaffResponse>(staff) : null;

            // await _elasticsearchService.IndexAsync(response, "tasks");
            return response;
        }

        public async Task<TaskResponse> AssignTaskAsync(Guid idTask, Guid idStaff, string assignedBy)
        {
            var existingTask = await _taskRepository.GetTaskByIdAsync(idTask);
            if (existingTask == null)
            {
                throw new TaskNotFoundException();
            }

            var staff = await _staffRepository.GetStaffByIdAsync(idStaff);
            if (staff == null)
            {
                throw new StaffNotFoundException();
            }

            existingTask.IdStaffAssigned = idStaff;
            existingTask.UpdatedAt = DateTime.UtcNow;

            var updatedTask = await _taskRepository.UpdateTaskAsync(existingTask);
            var response = _mapper.Map<TaskResponse>(updatedTask);
            response.StaffAssigned = _mapper.Map<StaffResponse>(staff);

            // await _elasticsearchService.IndexAsync(response, "tasks");

            // Create notification for staff
            var notification = new Notification
            {
                Title = "Bạn được giao công việc mới",
                Message = $"Bạn được giao công việc: {existingTask.Title}",
                Type = NotificationTypeConstant.NotificationTaskAssigned,
                IdStaff = idStaff,
                RelatedEntityType = "Task",
                RelatedEntityId = idTask
            };
            await _notificationRepository.AddNotificationAsync(notification);

            return response;
        }

        public async Task<TaskResponse> UpdateTaskStatusAsync(Guid idTask, int status, string updatedBy)
        {
            var existingTask = await _taskRepository.GetTaskByIdAsync(idTask);
            if (existingTask == null)
            {
                throw new TaskNotFoundException();
            }

            if (status < 0 || status > 3)
            {
                throw new ValidationException("Status không hợp lệ!");
            }

            existingTask.Status = status;
            existingTask.UpdatedAt = DateTime.UtcNow;

            var updatedTask = await _taskRepository.UpdateTaskAsync(existingTask);
            var staff = await _staffRepository.GetStaffByIdAsync(updatedTask.IdStaffAssigned);
            var response = _mapper.Map<TaskResponse>(updatedTask);
            response.StaffAssigned = staff != null ? _mapper.Map<StaffResponse>(staff) : null;

            // await _elasticsearchService.IndexAsync(response, "tasks");

            // Notify admin when task is completed
            if (status == 2) // COMPLETED
            {
                var adminStaff = await _staffRepository.GetStaffByRoleAsync("ADMIN");
                foreach (var admin in adminStaff)
                {
                    var notification = new Notification
                    {
                        Title = "Công việc hoàn thành",
                        Message = $"Công việc '{existingTask.Title}' đã được hoàn thành",
                        Type = NotificationTypeConstant.NotificationTaskCompleted,
                        IdStaff = admin.Id,
                        RelatedEntityType = "Task",
                        RelatedEntityId = idTask
                    };
                    await _notificationRepository.AddNotificationAsync(notification);
                }
            }

            return response;
        }

        public async Task<TaskResponse> GetTaskByIdAsync(Guid idTask)
        {
            var task = await _taskRepository.GetTaskByIdAsync(idTask);
            if (task == null)
            {
                throw new TaskNotFoundException();
            }
            return _mapper.Map<TaskResponse>(task);
        }

        private void ValidateTaskCreation(TaskCreationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new RequiredFieldException("Title");

            if (request.Title.Length > 200)
                throw new InvalidLengthException("Title", 1, 200);

            if (request.DueDate.HasValue && request.DueDate.Value < DateTime.UtcNow)
                throw new ValidationException("DueDate phải >= hiện tại!");

            if (request.Priority < 0 || request.Priority > 3)
                throw new ValidationException("Priority không hợp lệ!");

            if (!string.IsNullOrEmpty(request.LinkedEntityType) && !TaskLinkedEntityConstant.IsValid(request.LinkedEntityType))
                throw new ValidationException("LinkedEntityType không hợp lệ!");
        }

        private void ValidateTaskUpdate(TaskUpdateRequest request)
        {
            if (!string.IsNullOrEmpty(request.Title))
            {
                if (request.Title.Length > 200)
                    throw new InvalidLengthException("Title", 1, 200);
            }

            if (request.DueDate.HasValue && request.DueDate.Value < DateTime.UtcNow)
                throw new ValidationException("DueDate phải >= hiện tại!");

            if (request.Priority.HasValue && (request.Priority < 0 || request.Priority > 3))
                throw new ValidationException("Priority không hợp lệ!");

            if (request.Status.HasValue && (request.Status < 0 || request.Status > 3))
                throw new ValidationException("Status không hợp lệ!");
        }
    }

    public static class TaskStatusConstantExtensions
    {
        public static int ToStatusInt(this string status)
        {
            return status.ToUpper() switch
            {
                "PENDING" => 0,
                "IN_PROGRESS" => 1,
                "COMPLETED" => 2,
                "CANCELLED" => 3,
                _ => 0
            };
        }
    }
}