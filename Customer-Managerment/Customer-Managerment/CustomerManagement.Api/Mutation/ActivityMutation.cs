using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.Activities;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class ActivityMutation
    {
        private readonly ActivityHandler _activityHandler;

        public ActivityMutation(ActivityHandler activityHandler)
        {
            _activityHandler = activityHandler;
        }

        [Authorize]
        public async Task<ActivityResponse> CreateActivityAsync(ActivityCreationRequest request)
        {
            return await _activityHandler.CreateActivityAsync(request);
        }

        [Authorize]
        public async Task<ActivityResponse> UpdateActivityAsync(ActivityUpdateRequest request, Guid idActivity)
        {
            return await _activityHandler.UpdateActivityAsync(request, idActivity);
        }

        [Authorize]
        public async Task<string> DeleteActivityAsync(Guid idActivity)
        {
            return await _activityHandler.DeleteActivityAsync(idActivity);
        }
    }
}
