using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.Tasks;
using Customer_Managerment.CustomerManagement.Application.UseCases.Users;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class UserMutation
    {
        private readonly UsersHandler _usersHandler;

        public UserMutation(UsersHandler usersHandler)
        {
            _usersHandler = usersHandler;
        }

        public async Task<UserResponse> CreateUserAsync(UserCreationRequest userCreationRequest)
        {
            return await _usersHandler.CreateUserAsync(userCreationRequest);
        }

        [Authorize]
        public async Task<UserResponse> UpdateUserAsync(UserUpdateRequest userUpdateRequest, Guid idUser)
        {
            return await _usersHandler.UpdateUserAsync(userUpdateRequest, idUser);
        }

        [Authorize]
        public async Task<string> DeleteUserAsync(Guid idUser)
        {
            return await _usersHandler.DeleteUserAsync(idUser);
        }
    }
}
