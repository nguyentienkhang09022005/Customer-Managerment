using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    [Authorize]
    public class TeamAssignmentQuery
    {
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IMapper _mapper;

        public TeamAssignmentQuery(
            ITeamMemberRepository teamMemberRepository,
            IStaffRepository staffRepository,
            IMapper mapper)
        {
            _teamMemberRepository = teamMemberRepository;
            _staffRepository = staffRepository;
            _mapper = mapper;
        }

        [UseFiltering]
        [UseSorting]
        public async Task<List<TeamMemberResponse>> GetTeamMembersAsync(string entityType, Guid entityId)
        {
            var members = await _teamMemberRepository.GetByEntityAsync(entityType, entityId);
            var responses = new List<TeamMemberResponse>();

            foreach (var member in members)
            {
                var staff = await _staffRepository.GetStaffByIdAsync(member.IdStaff);
                var response = new TeamMemberResponse
                {
                    Id = member.Id,
                    EntityType = member.EntityType,
                    EntityId = member.EntityId,
                    IdStaff = member.IdStaff,
                    Staff = staff != null ? MapStaffToResponse(staff) : null,
                    Role = TeamRoleConstant.ToString(member.Role),
                    AssignedAt = member.AssignedAt,
                    AssignedBy = member.AssignedBy,
                    CanEdit = member.CanEdit,
                    CanDelete = member.CanDelete
                };
                responses.Add(response);
            }

            return responses;
        }

        [UseFiltering]
        [UseSorting]
        public async Task<List<TeamMemberResponse>> GetMyTeamsAsync(Guid idStaff)
        {
            var teams = await _teamMemberRepository.GetByStaffAsync(idStaff);
            var responses = new List<TeamMemberResponse>();

            foreach (var team in teams)
            {
                var staff = await _staffRepository.GetStaffByIdAsync(team.IdStaff);
                var response = new TeamMemberResponse
                {
                    Id = team.Id,
                    EntityType = team.EntityType,
                    EntityId = team.EntityId,
                    IdStaff = team.IdStaff,
                    Staff = staff != null ? MapStaffToResponse(staff) : null,
                    Role = TeamRoleConstant.ToString(team.Role),
                    AssignedAt = team.AssignedAt,
                    AssignedBy = team.AssignedBy,
                    CanEdit = team.CanEdit,
                    CanDelete = team.CanDelete
                };
                responses.Add(response);
            }

            return responses;
        }

        public async Task<TeamMemberResponse?> GetTeamMemberPermissionsAsync(Guid idTeamMember)
        {
            var member = await _teamMemberRepository.GetByIdAsync(idTeamMember);
            if (member == null)
                return null;

            var staff = await _staffRepository.GetStaffByIdAsync(member.IdStaff);
            return new TeamMemberResponse
            {
                Id = member.Id,
                EntityType = member.EntityType,
                EntityId = member.EntityId,
                IdStaff = member.IdStaff,
                Staff = staff != null ? MapStaffToResponse(staff) : null,
                Role = TeamRoleConstant.ToString(member.Role),
                AssignedAt = member.AssignedAt,
                AssignedBy = member.AssignedBy,
                CanEdit = member.CanEdit,
                CanDelete = member.CanDelete
            };
        }

        private static StaffResponse MapStaffToResponse(Person staff)
        {
            return new StaffResponse
            {
                Id = staff.Id,
                Username = staff.Username ?? "",
                Role = staff.Role,
                Salary = staff.Salary,
                CreatedAt = staff.CreatedAt,
                UpdatedAt = staff.UpdatedAt,
                Person = new PersonResponse
                {
                    Fullname = staff.Fullname,
                    Email = staff.Email,
                    Phone = staff.Phone,
                    Location = staff.Location
                }
            };
        }
    }
}