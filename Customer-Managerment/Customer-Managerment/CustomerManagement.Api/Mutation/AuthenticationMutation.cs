using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.Authen;
using Customer_Managerment.CustomerManagement.Application.Handlers.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class AuthenticationMutation
    {
        private readonly AuthenticationHandler _authenticationHandler;

        public AuthenticationMutation(AuthenticationHandler authenticationHandler)
        {
            _authenticationHandler = authenticationHandler;
        }

        [AllowAnonymous]
        [EnableRateLimiting("login-attempts")]
        public async Task<AuthenticationResponse> LoginAsync(AuthenticationRequest authenticationRequest)
        {
            return await _authenticationHandler.LoginHandleAsync(authenticationRequest);
        }

        [Authorize]
        public async Task<string> LogoutAsync()
        {
            return await _authenticationHandler.LogoutHandleAsync();
        }

        [AllowAnonymous]
        [EnableRateLimiting("login-attempts")]
        public async Task<AuthenticationResponse> RefreshTokenAsync()
        {
            return await _authenticationHandler.RefreshTokenHandleAsync();
        }

        [Authorize]
        public async Task<IntrospectResponse> IntrospectAsync(IntrospectRequest introspectRequest)
        {
            return await _authenticationHandler.IntrospectTokenHandleAsync(introspectRequest);
        }


    }
}
