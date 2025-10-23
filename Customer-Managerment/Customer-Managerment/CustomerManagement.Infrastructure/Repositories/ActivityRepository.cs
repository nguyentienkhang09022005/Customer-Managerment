using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public ActivityRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<Activity> AddActivityAsync(ActivityDomain activityDomain)
        {
            await using var context = _contextFactory.CreateDbContext();
            var activity = _mapper.Map<Activity>(activityDomain);
            activity.IdActivity = Guid.NewGuid();

            await context.Activities.AddAsync(activity);
            await context.SaveChangesAsync();
            return activity;
        }

        public async Task DeleteActivityAsync(Guid idActivity)
        {
            await using var context = _contextFactory.CreateDbContext();
            var activity = await context.Activities.FindAsync(idActivity);
            if (activity == null)
                throw new NotFoundException("Không tìm thấy hoạt động!");

            context.Activities.Remove(activity);
            await context.SaveChangesAsync();
        }

        public async Task<List<Activity>> GetListActivitiesAsync(Guid idUser)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Activities
                .AsNoTracking()
                .Where(a => a.IdUser == idUser)
                .Include(d => d.IdUserNavigation)
                .Include(a => a.IdCustomerNavigation)
                .ToListAsync();
        }

        public async Task<Activity> GetActivityByIdAsync(Guid idActivity)
        {
            await using var context = _contextFactory.CreateDbContext();
            var activity = await context.Activities
                .Include(d => d.IdUserNavigation)
                .Include(a => a.IdCustomerNavigation)
                .FirstOrDefaultAsync(a => a.IdActivity == idActivity);

            if (activity == null)
                throw new NotFoundException("Không tìm thấy thông tin hoạt động!");

            return activity;
        }

        public async Task<Activity> UpdateActivityAsync(ActivityDomain activityDomain, Activity activity)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.Attach(activity);

            _mapper.Map(activityDomain, activity);
            context.Entry(activity).State = EntityState.Modified;

            await context.SaveChangesAsync();
            return activity;
        }

        public async Task<Activity> GetExistingActivityAsync(Guid idActivity)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Activities
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .FirstOrDefaultAsync(a => a.IdActivity == idActivity);
        }
    }
}
