using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.UseCases.Authen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class ForgotPasswordMutation
    {
        private readonly ForgotPasswordHandler _forgotPasswordHandler;

        public ForgotPasswordMutation(ForgotPasswordHandler forgotPasswordHandler)
        {
            _forgotPasswordHandler = forgotPasswordHandler;
        }

        [AllowAnonymous]
        [EnableRateLimiting("otp-attempts")]
        public async Task<string> SendOTPForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest)
        {
            return await _forgotPasswordHandler.SendOTPForForgotPasswordHandleAsync(forgotPasswordRequest);
        }

        [AllowAnonymous]
        [EnableRateLimiting("otp-attempts")]
        public async Task<string> ConfirmOTPForgotPasswordAsync(ChangePasswordRequest changePasswordRequest)
        {
            return await _forgotPasswordHandler.ConfirmOTPForForgotPasswordHandleAsync(changePasswordRequest);
        }
    }
}
