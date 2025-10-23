using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IActivityRepository
    {
        Task<List<Activity>> GetListActivitiesAsync(Guid idUser);

        Task<Activity> AddActivityAsync(ActivityDomain activityDomain);

        Task<Activity> GetActivityByIdAsync(Guid idActivity);

        Task<Activity> UpdateActivityAsync(ActivityDomain activityDomain, Activity activity);

        Task DeleteActivityAsync(Guid idActivity);

        Task<Activity> GetExistingActivityAsync(Guid idActivity);
    }
}
