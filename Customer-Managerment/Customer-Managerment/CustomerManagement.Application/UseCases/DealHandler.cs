using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class DealHandler
    {
        private readonly IDealRepository _dealRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IMapper _mapper;

        private static readonly string[] ValidDealStatuses = { "OPEN", "NEGOTIATING", "WON", "LOST" };

        public DealHandler(IDealRepository dealRepository,
                           IStaffRepository staffRepository,
                           ICustomerRepository customerRepository,
                           ITeamMemberRepository teamMemberRepository,
                           IMapper mapper)
        {
            _dealRepository = dealRepository;
            _staffRepository = staffRepository;
            _customerRepository = customerRepository;
            _teamMemberRepository = teamMemberRepository;
            _mapper = mapper;
        }

        public async Task<DealResponse> CreateDealAsync(DealCreationRequest request)
        {
            ValidateDealCreation(request);

            var staff = await _staffRepository.GetStaffByIdAsync(request.IdStaff);
            if (staff == null)
            {
                throw new StaffNotFoundException();
            }

            var customer = await _customerRepository.GetCustomerByIdAsync(request.IdCustomer);
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }

            var deal = _mapper.Map<Deal>(request);
            deal.IdStaff = request.IdStaff;
            deal.IdCustomer = request.IdCustomer;

            var createdDeal = await _dealRepository.AddDealAsync(deal);

            // Auto-add creator as OWNER in team_members
            var teamMember = new TeamMember
            {
                Id = Guid.NewGuid(),
                EntityType = TeamEntityTypeConstant.EntityTypeDeal,
                EntityId = createdDeal.IdDeal,
                IdStaff = request.IdStaff,
                Role = TeamRoleConstant.FromString(TeamRoleConstant.RoleOwner),
                AssignedAt = DateTime.UtcNow,
                AssignedBy = "system",
                CanEdit = true,
                CanDelete = true
            };
            await _teamMemberRepository.AddAsync(teamMember);

            var response = _mapper.Map<DealResponse>(createdDeal);
            response.Customer = _mapper.Map<CustomerResponse>(customer);
            response.Staff = _mapper.Map<StaffResponse>(staff);

            // await _elasticsearchService.IndexAsync(response, "deals");

            return response;
        }

        public async Task<string> DeleteDealAsync(Guid idDeal)
        {
            var result = await _dealRepository.SoftDeleteDealAsync(idDeal);
            if (!result)
            {
                throw new DealNotFoundException();
            }

            // Cleanup team_members when deal is deleted
            await _teamMemberRepository.RemoveByEntityAsync(TeamEntityTypeConstant.EntityTypeDeal, idDeal);

            // await _elasticsearchService.DeleteAsync<DealResponse>(idDeal.ToString(), "deals");
            return "Xóa deal thành công!";
        }

        public async Task<DealResponse> UpdateDealAsync(DealUpdateRequest request, Guid idDeal)
        {
            var existingDeal = await _dealRepository.GetDealByIdAsync(idDeal);
            if (existingDeal == null)
            {
                throw new DealNotFoundException();
            }

            ValidateDealUpdate(request);

            existingDeal.Title = request.Title ?? existingDeal.Title;
            existingDeal.Content = request.Content ?? existingDeal.Content;
            existingDeal.Price = request.Price ?? existingDeal.Price;

            if (!string.IsNullOrEmpty(request.Status))
            {
                if (!ValidDealStatuses.Contains(request.Status.ToUpper()))
                {
                    throw new ValidationException($"Status '{request.Status}' không hợp lệ!");
                }
                existingDeal.Status = request.Status.ToUpper();
            }

            existingDeal.UpdatedAt = DateTime.UtcNow;

            var updatedDeal = await _dealRepository.UpdateDealAsync(existingDeal);

            var staff = await _staffRepository.GetStaffByIdAsync(updatedDeal.IdStaff);
            var customer = await _customerRepository.GetCustomerByIdAsync(updatedDeal.IdCustomer);

            var response = _mapper.Map<DealResponse>(updatedDeal);
            response.Customer = _mapper.Map<CustomerResponse>(customer);
            response.Staff = _mapper.Map<StaffResponse>(staff);

            // await _elasticsearchService.IndexAsync(response, "deals");

            return response;
        }

        public async Task<DealResponse> GetDealByIdAsync(Guid idDeal)
        {
            var deal = await _dealRepository.GetDealByIdAsync(idDeal);
            if (deal == null)
            {
                throw new DealNotFoundException();
            }

            var response = _mapper.Map<DealResponse>(deal);
            response.Customer = _mapper.Map<CustomerResponse>(deal.IdCustomerNavigation);
            response.Staff = _mapper.Map<StaffResponse>(deal.IdStaffNavigation);

            return response;
        }

        private void ValidateDealCreation(DealCreationRequest request)
        {
            if (request.IdStaff == Guid.Empty)
                throw new InvalidGuidException("IdStaff");

            if (request.IdCustomer == Guid.Empty)
                throw new InvalidGuidException("IdCustomer");

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new RequiredFieldException("Title");

            if (request.Title?.Length > 100)
                throw new InvalidLengthException("Title", 1, 100);

            if (request.Price.HasValue && request.Price < 0)
                throw new ValidationException("Giá không được âm!");
        }

        private void ValidateDealUpdate(DealUpdateRequest request)
        {
            if (!string.IsNullOrEmpty(request.Title))
            {
                if (request.Title.Length > 100)
                    throw new InvalidLengthException("Title", 1, 100);
            }

            if (request.Price.HasValue && request.Price < 0)
                throw new ValidationException("Giá không được âm!");
        }
    }
}