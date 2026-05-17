using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class StaffPresenceMutation
    {
        private readonly StaffPresenceHandler _handler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StaffPresenceMutation(StaffPresenceHandler handler, IHttpContextAccessor httpContextAccessor)
        {
            _handler = handler;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<StaffStatusResponse> UpdateMyStatusAsync(int status)
        {
            var idStaff = GetCurrentUserId();
            var ipAddress = GetIpAddress();
            var userAgent = GetUserAgent();

            return await _handler.UpdateMyStatusAsync(idStaff, status, ipAddress, userAgent);
        }

        public async Task<StaffStatusResponse> RefreshLastActiveAsync()
        {
            var idStaff = GetCurrentUserId();
            var ipAddress = GetIpAddress();
            var userAgent = GetUserAgent();

            return await _handler.RefreshLastActiveAsync(idStaff, ipAddress, userAgent);
        }

        private Guid GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirst("sub")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        private string? GetIpAddress()
        {
            return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        }

        private string? GetUserAgent()
        {
            return _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].FirstOrDefault();
        }
    }
}