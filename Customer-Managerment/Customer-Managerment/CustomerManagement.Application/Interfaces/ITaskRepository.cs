using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskEntity?> GetTaskByIdAsync(Guid idTask);
        Task<IQueryable<TaskEntity>> GetListTaskAsync();
        Task<IQueryable<TaskEntity>> GetTasksByStaffAsync(Guid idStaff);
        Task<IQueryable<TaskEntity>> GetTasksByStatusAsync(int status);
        Task<TaskEntity> AddTaskAsync(TaskEntity task);
        Task<TaskEntity> UpdateTaskAsync(TaskEntity task);
        Task<bool> SoftDeleteTaskAsync(Guid idTask);
        Task<bool> RestoreTaskAsync(Guid idTask);
        Task<int> GetTotalTasksAsync();
        Task<(List<TaskEntity> Items, int TotalCount)> GetTasksPagedAsync(int page, int pageSize);
    }
}