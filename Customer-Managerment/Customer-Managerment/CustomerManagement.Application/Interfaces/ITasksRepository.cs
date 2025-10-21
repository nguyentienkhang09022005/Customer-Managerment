using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface ITasksRepository
    {
        Task<List<Tasks>> GetListTasksAsync(Guid idUser);

        Task<Tasks> AddTasksAsync(TasksDomain tasksDomain);

        Task<Tasks> GetTasksByIdAsync(Guid idTask);

        Task<Tasks> UpdateTaskAsync(TasksDomain tasksDomain, Tasks tasks);

        Task DeleteTaskAsync(Guid idTask);

        Task<bool> CheckTaskExistsAsync(Guid idTask);

        Task<Tasks> GetExistingTaskAsync(Guid idTask);

    }
}
