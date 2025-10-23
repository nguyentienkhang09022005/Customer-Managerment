using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.Activities;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class ActivityQuery
    {
        private readonly ActivityHandler _activityHandler;

        public ActivityQuery(ActivityHandler activityHandler)
        {
            _activityHandler = activityHandler;
        }

        //[Authorize]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public async Task<List<ActivityResponse>> GetActivitiesAsync(Guid idUser)
        {
            return await _activityHandler.GetListActivitiesAsync(idUser);
        }

        //[Authorize]
        public async Task<ActivityResponse> GetInfActivityAsync(Guid idActivity)
        {
            return await _activityHandler.GetInfActivityAsync(idActivity);
        }
    }
}
