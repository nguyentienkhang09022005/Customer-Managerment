using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class TeamAssignmentHandler
    {
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ILeadRepository _leadRepository;
        private readonly IDealRepository _dealRepository;
        private readonly IMapper _mapper;

        public TeamAssignmentHandler(
            ITeamMemberRepository teamMemberRepository,
            IStaffRepository staffRepository,
            ILeadRepository leadRepository,
            IDealRepository dealRepository,
            IMapper mapper)
        {
            _teamMemberRepository = teamMemberRepository;
            _staffRepository = staffRepository;
            _leadRepository = leadRepository;
            _dealRepository = dealRepository;
            _mapper = mapper;
        }

        public async Task<TeamMemberResponse> AddTeamMemberAsync(AddTeamMemberRequest request)
        {
            ValidateAddRequest(request);

            var staff = await _staffRepository.GetStaffByIdAsync(request.IdStaff);
            if (staff == null)
            {
                throw new StaffNotFoundException();
            }

            await ValidateEntityExistsAsync(request.EntityType, request.EntityId);

            var existing = await _teamMemberRepository.GetTeamMemberAsync(request.EntityType, request.EntityId, request.IdStaff);
            if (existing != null)
            {
                throw new ConflictException("Staff đã là thành viên của nhóm này!");
            }

            var teamMember = new TeamMember
            {
                EntityType = request.EntityType,
                EntityId = request.EntityId,
                IdStaff = request.IdStaff,
                Role = TeamRoleConstant.FromString(request.Role),
                AssignedBy = request.AssignedBy,
                CanEdit = request.CanEdit,
                CanDelete = request.CanDelete,
                AssignedAt = DateTime.UtcNow
            };

            var created = await _teamMemberRepository.AddAsync(teamMember);
            var response = _mapper.Map<TeamMemberResponse>(created);
            response.Staff = MapStaffToResponse(staff);
            return response;
        }

        public async Task<TeamMemberResponse> UpdateTeamMemberAsync(Guid idTeamMember, UpdateTeamMemberRequest request)
        {
            var teamMember = await _teamMemberRepository.GetByIdAsync(idTeamMember);
            if (teamMember == null)
            {
                throw new TeamMemberNotFoundException();
            }

            if (request.Role.HasValue)
                teamMember.Role = request.Role.Value;
            if (request.CanEdit.HasValue)
                teamMember.CanEdit = request.CanEdit.Value;
            if (request.CanDelete.HasValue)
                teamMember.CanDelete = request.CanDelete.Value;

            var updated = await _teamMemberRepository.UpdateAsync(teamMember);
            var staff = await _staffRepository.GetStaffByIdAsync(updated.IdStaff);
            var response = _mapper.Map<TeamMemberResponse>(updated);
            response.Staff = staff != null ? MapStaffToResponse(staff) : null;
            return response;
        }

        public async Task<bool> RemoveTeamMemberAsync(Guid idTeamMember)
        {
            var teamMember = await _teamMemberRepository.GetByIdAsync(idTeamMember);
            if (teamMember == null)
            {
                throw new TeamMemberNotFoundException();
            }

            if (teamMember.Role == 0)
            {
                var count = await _teamMemberRepository.CountByEntityAsync(teamMember.EntityType, teamMember.EntityId);
                if (count <= 1)
                {
                    throw new BusinessRuleException("Không thể xóa OWNER cuối cùng!");
                }
            }

            return await _teamMemberRepository.RemoveAsync(idTeamMember);
        }

        public async Task<TeamMemberResponse> TransferOwnershipAsync(string entityType, Guid entityId, Guid newOwnerId)
        {
            await ValidateEntityExistsAsync(entityType, entityId);

            var newOwner = await _staffRepository.GetStaffByIdAsync(newOwnerId);
            if (newOwner == null)
            {
                throw new StaffNotFoundException();
            }

            var existingOwner = await _teamMemberRepository.GetTeamMemberAsync(entityType, entityId, newOwnerId);
            if (existingOwner == null)
            {
                throw new NotFoundException("OWNER mới phải là thành viên của nhóm!");
            }

            var members = await _teamMemberRepository.GetByEntityAsync(entityType, entityId);
            foreach (var member in members)
            {
                if (member.Role == 0)
                {
                    member.Role = TeamRoleConstant.FromString("MEMBER");
                    await _teamMemberRepository.UpdateAsync(member);
                }
            }

            existingOwner.Role = 0;
            existingOwner.CanEdit = true;
            existingOwner.CanDelete = true;
            var updated = await _teamMemberRepository.UpdateAsync(existingOwner);

            var response = _mapper.Map<TeamMemberResponse>(updated);
            response.Staff = MapStaffToResponse(newOwner);
            return response;
        }

        public async Task<List<TeamMemberResponse>> GetTeamMembersAsync(string entityType, Guid entityId)
        {
            var members = await _teamMemberRepository.GetByEntityAsync(entityType, entityId);
            var responses = new List<TeamMemberResponse>();

            foreach (var member in members)
            {
                var staff = await _staffRepository.GetStaffByIdAsync(member.IdStaff);
                var response = _mapper.Map<TeamMemberResponse>(member);
                response.Staff = staff != null ? MapStaffToResponse(staff) : null;
                responses.Add(response);
            }

            return responses;
        }

        public async Task<List<TeamMemberResponse>> GetMyTeamsAsync(Guid idStaff)
        {
            var teams = await _teamMemberRepository.GetByStaffAsync(idStaff);
            var responses = new List<TeamMemberResponse>();

            foreach (var team in teams)
            {
                var staff = await _staffRepository.GetStaffByIdAsync(team.IdStaff);
                var response = _mapper.Map<TeamMemberResponse>(team);
                response.Staff = staff != null ? MapStaffToResponse(staff) : null;
                responses.Add(response);
            }

            return responses;
        }

        private void ValidateAddRequest(AddTeamMemberRequest request)
        {
            if (!TeamEntityTypeConstant.IsValid(request.EntityType))
            {
                throw new ValidationException("EntityType không hợp lệ!");
            }

            if (!TeamRoleConstant.IsValid(request.Role))
            {
                throw new ValidationException("Role không hợp lệ!");
            }
        }

        private async Task ValidateEntityExistsAsync(string entityType, Guid entityId)
        {
            if (entityType == "Lead")
            {
                var lead = await _leadRepository.GetLeadByIdAsync(entityId);
                if (lead == null)
                {
                    throw new LeadNotFoundException();
                }
            }
            else if (entityType == "Deal")
            {
                var deal = await _dealRepository.GetDealByIdAsync(entityId);
                if (deal == null)
                {
                    throw new DealNotFoundException();
                }
            }
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