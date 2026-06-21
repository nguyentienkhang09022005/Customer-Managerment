using Customer_Managerment.CustomerManagement.Api.Input.Type;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    [Authorize]
    public class TeamAssignmentMutation
    {
        private readonly TeamAssignmentHandler _handler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TeamAssignmentMutation(TeamAssignmentHandler handler, IHttpContextAccessor httpContextAccessor)
        {
            _handler = handler;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TeamMemberResponse> AddTeamMemberAsync(AddTeamMemberInput input)
        {
            var assignedBy = GetCurrentUserName();
            var request = new AddTeamMemberRequest
            {
                EntityType = input.EntityType,
                EntityId = input.EntityId,
                IdStaff = input.IdStaff,
                Role = input.Role.ToString(),
                AssignedBy = assignedBy,
                CanEdit = input.CanEdit,
                CanDelete = input.CanDelete
            };

            return await _handler.AddTeamMemberAsync(request);
        }

        public async Task<TeamMemberResponse> UpdateTeamMemberAsync(UpdateTeamMemberInput input, Guid idTeamMember)
        {
            var request = new UpdateTeamMemberRequest
            {
                Role = input.Role.HasValue ? (int)input.Role.Value : null,
                CanEdit = input.CanEdit,
                CanDelete = input.CanDelete
            };

            return await _handler.UpdateTeamMemberAsync(idTeamMember, request);
        }

        public async Task<bool> RemoveTeamMemberAsync(Guid idTeamMember)
        {
            await _handler.RemoveTeamMemberAsync(idTeamMember);
            return true;
        }

        public async Task<TeamMemberResponse> TransferOwnershipAsync(string entityType, Guid entityId, Guid newOwnerId)
        {
            return await _handler.TransferOwnershipAsync(entityType, entityId, newOwnerId);
        }

        private string? GetCurrentUserName()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}