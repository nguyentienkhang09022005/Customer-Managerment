using Customer_Managerment.CustomerManagement.Api.Input.Type;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class StaffMutation
    {
        private readonly StaffHandler _staffHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StaffMutation(StaffHandler staffHandler, IHttpContextAccessor httpContextAccessor)
        {
            _staffHandler = staffHandler;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<StaffResponse> CreateStaffAsync(StaffInput input)
        {
            var request = new StaffCreationRequest
            {
                Fullname = input.Fullname,
                Email = input.Email,
                Phone = input.Phone,
                Location = input.Location,
                Username = input.Username,
                Password = input.Password,
                Role = input.Role.ToString(),
                Salary = input.Salary
            };

            return await _staffHandler.CreateStaffAsync(request);
        }

        public async Task<StaffResponse> UpdateStaffAsync(StaffUpdateInput input, Guid idStaff)
        {
            var request = new StaffUpdateRequest
            {
                Fullname = input.Fullname,
                Email = input.Email,
                Phone = input.Phone,
                Location = input.Location,
                Role = input.Role?.ToString(),
                Salary = input.Salary
            };

            return await _staffHandler.UpdateStaffAsync(request, idStaff);
        }

        public async Task<bool> DeleteStaffAsync(Guid idStaff)
        {
            await _staffHandler.DeleteStaffAsync(idStaff);
            return true;
        }

        public async Task<StaffResponse> RestoreStaffAsync(Guid idStaff)
        {
            return await _staffHandler.RestoreStaffAsync(idStaff);
        }

        private string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        }
    }
}