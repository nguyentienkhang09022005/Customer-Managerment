using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.UseCases.Authen;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class RegisterMutation
    {
        private readonly RegisterHandler _registerHandler;

        public RegisterMutation(RegisterHandler registerHandler)
        {
            _registerHandler = registerHandler;
        }

        // Register Mutation
        public async Task<string> RegisterAsync(RegisterRequest registerRequest)
        {
            return await _registerHandler.SendOtpToRegisterAsync(registerRequest);
        }
    }
}
