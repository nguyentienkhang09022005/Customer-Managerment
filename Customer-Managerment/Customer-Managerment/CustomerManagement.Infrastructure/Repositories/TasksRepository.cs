using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{

    public class TasksRepository : ITasksRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        private static string DefaultStatusTask = "Pending";

        public TasksRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<Tasks> AddTasksAsync(TasksDomain tasksDomain)
        {
            await using var context = _contextFactory.CreateDbContext();
            var task = _mapper.Map<Tasks>(tasksDomain);

            task.IdTask = Guid.NewGuid();
            task.Status = DefaultStatusTask;
            await context.Tasks.AddAsync(task);
            await context.SaveChangesAsync();
            return task;
        }

        public async Task DeleteTaskAsync(Guid idTask)
        {
            await using var context = _contextFactory.CreateDbContext();
            var task = await context.Tasks.FindAsync(idTask);
            if (task == null)
                throw new NotFoundException("Không tìm thấy công việc!");

            context.Tasks.Remove(task);
            await context.SaveChangesAsync();
        }

        public async Task<List<Tasks>> GetListTasksAsync(Guid idUser)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Tasks
                .AsNoTracking()
                .Where(u  => u.IdUser == idUser)
                .IgnoreAutoIncludes()
                .ToListAsync();
        }

        public async Task<Tasks> GetTasksByIdAsync(Guid idTask)
        {
            await using var context = _contextFactory.CreateDbContext();
            var task = await context.Tasks
                    .IgnoreAutoIncludes()
                    .FirstOrDefaultAsync(t => t.IdTask == idTask);

            if (task == null)
                throw new NotFoundException("Không tìm thấy thông tin công việc!");
            return task;
        }

        public async Task<Tasks> UpdateTaskAsync(TasksDomain tasksDomain, Tasks task)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.Attach(task);

            _mapper.Map(tasksDomain, task);

            context.Entry(task).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> CheckTaskExistsAsync(Guid idTask)
        {
            await using var context = _contextFactory.CreateDbContext();

            return await context.Tasks
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .AnyAsync(t => t.IdTask == idTask);
        }

        public async Task<Tasks> GetExistingTaskAsync(Guid idTask)
        {
            await using var context = _contextFactory.CreateDbContext();

            var task = await context.Tasks
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .FirstOrDefaultAsync(t => t.IdTask == idTask);

            return task;
        }
    }
}
