using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public TaskRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<TaskEntity?> GetTaskByIdAsync(Guid idTask)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Tasks
                .Include(t => t.IdStaffAssignedNavigation)
                .FirstOrDefaultAsync(t => t.IdTask == idTask);
        }

        public async Task<IQueryable<TaskEntity>> GetListTaskAsync()
        {
            var context = _contextFactory.CreateDbContext();
            return context.Tasks
                .Include(t => t.IdStaffAssignedNavigation)
                .Where(t => !t.IsDeleted)
                .AsNoTracking();
        }

        public async Task<IQueryable<TaskEntity>> GetTasksByStaffAsync(Guid idStaff)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Tasks
                .Include(t => t.IdStaffAssignedNavigation)
                .Where(t => t.IdStaffAssigned == idStaff && !t.IsDeleted)
                .AsNoTracking();
        }

        public async Task<IQueryable<TaskEntity>> GetTasksByStatusAsync(int status)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Tasks
                .Include(t => t.IdStaffAssignedNavigation)
                .Where(t => t.Status == status && !t.IsDeleted)
                .AsNoTracking();
        }

        public async Task<TaskEntity> AddTaskAsync(TaskEntity task)
        {
            await using var context = _contextFactory.CreateDbContext();

            task.IdTask = Guid.NewGuid();
            task.CreatedAt = DateTime.UtcNow;
            task.IsDeleted = false;

            await context.Tasks.AddAsync(task);
            await context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskEntity> UpdateTaskAsync(TaskEntity task)
        {
            await using var context = _contextFactory.CreateDbContext();
            var existingTask = await context.Tasks.FirstOrDefaultAsync(t => t.IdTask == task.IdTask);

            if (existingTask == null)
                throw new NotFoundException("Không tìm thấy Task!");

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.DueDate = task.DueDate;
            existingTask.Priority = task.Priority;
            existingTask.Status = task.Status;
            existingTask.IdStaffAssigned = task.IdStaffAssigned;
            existingTask.LinkedEntityType = task.LinkedEntityType;
            existingTask.LinkedEntityId = task.LinkedEntityId;
            existingTask.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return existingTask;
        }

        public async Task<bool> SoftDeleteTaskAsync(Guid idTask)
        {
            await using var context = _contextFactory.CreateDbContext();
            var task = await context.Tasks.FirstOrDefaultAsync(t => t.IdTask == idTask);

            if (task == null)
                return false;

            task.IsDeleted = true;
            task.DeletedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreTaskAsync(Guid idTask)
        {
            await using var context = _contextFactory.CreateDbContext();
            var task = await context.Tasks
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.IdTask == idTask);

            if (task == null || !task.IsDeleted)
                return false;

            task.IsDeleted = false;
            task.DeletedAt = null;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalTasksAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Tasks.CountAsync(t => !t.IsDeleted);
        }
    }
}