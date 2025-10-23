using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class UserQuery
    {
        private readonly UsersHandler _usersHandler;

        public UserQuery(UsersHandler usersHandler)
        {
            _usersHandler = usersHandler;
        }

        // User Query
        [Authorize]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public async Task<List<UserResponse>> GetUsersAsync()
        {
            return await _usersHandler.GetListUsersAsync();
        }

        [Authorize]
        public async Task<UserResponse> GetInfUsersAsync(Guid idTask)
        {
            return await _usersHandler.GetInfUserAsync(idTask);
        }

        //[Authorize]
        public async Task<List<UserResponse>> SearchUsersAsync(string keyword)
        {
            return await _usersHandler.SearchUsersAsync(keyword);
        }
    }
}
