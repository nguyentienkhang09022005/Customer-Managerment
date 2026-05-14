using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class StaffHandler
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly IMapper _mapper;

        public StaffHandler(
            IStaffRepository staffRepository,
            IElasticsearchService elasticsearchService,
            IMapper mapper)
        {
            _staffRepository = staffRepository;
            _elasticsearchService = elasticsearchService;
            _mapper = mapper;
        }

        public async Task<StaffResponse> CreateStaffAsync(StaffCreationRequest request)
        {
            // Validate input
            ValidateStaffCreation(request);

            // Check email uniqueness
            var checkEmail = await _staffRepository.GetStaffByEmailAsync(request.Email);
            if (checkEmail != null)
            {
                throw new EmailAlreadyExistsException();
            }

            // Check username uniqueness
            var checkUsername = await _staffRepository.GetStaffByUsernameAsync(request.Username);
            if (checkUsername != null)
            {
                throw new UsernameAlreadyExistsException();
            }

            // Map request to Person entity
            var staff = _mapper.Map<Person>(request);

            // Create staff
            var createdStaff = await _staffRepository.AddStaffAsync(staff);

            // Map to response
            var response = _mapper.Map<StaffResponse>(createdStaff);

            // Index to Elasticsearch
            await _elasticsearchService.IndexAsync(response, "staffs");

            return response;
        }

        public async Task<string> DeleteStaffAsync(Guid idStaff)
        {
            var result = await _staffRepository.SoftDeleteStaffAsync(idStaff);
            if (!result)
            {
                throw new StaffNotFoundException();
            }

            // Remove from Elasticsearch
            await _elasticsearchService.DeleteAsync<StaffResponse>(idStaff.ToString(), "staffs");

            return "Xóa nhân viên thành công!";
        }

        public async Task<StaffResponse> RestoreStaffAsync(Guid idStaff)
        {
            var result = await _staffRepository.RestoreStaffAsync(idStaff);
            if (!result)
            {
                throw new StaffNotFoundException();
            }

            var staff = await _staffRepository.GetStaffByIdAsync(idStaff);
            var response = _mapper.Map<StaffResponse>(staff);

            // Re-index to Elasticsearch
            await _elasticsearchService.IndexAsync(response, "staffs");

            return response;
        }

        public async Task<StaffResponse> UpdateStaffAsync(StaffUpdateRequest request, Guid idStaff)
        {
            ValidateStaffUpdate(request);

            var existingStaff = await _staffRepository.GetStaffByIdAsync(idStaff);
            if (existingStaff == null)
            {
                throw new StaffNotFoundException();
            }

            // Check email uniqueness (excluding current staff)
            var checkEmail = await _staffRepository.GetStaffByEmailAsync(request.Email);
            if (checkEmail != null && checkEmail.Id != idStaff)
            {
                throw new EmailAlreadyExistsException();
            }

            // Update fields
            existingStaff.Fullname = request.Fullname;
            existingStaff.Email = request.Email;
            existingStaff.Phone = request.Phone;
            existingStaff.Role = request.Role;
            existingStaff.Salary = request.Salary;
            existingStaff.UpdatedAt = DateTime.UtcNow;

            var updatedStaff = await _staffRepository.UpdateStaffAsync(existingStaff);
            var response = _mapper.Map<StaffResponse>(updatedStaff);

            // Re-index to Elasticsearch
            await _elasticsearchService.IndexAsync(response, "staffs");

            return response;
        }

        public async Task<StaffResponse> GetStaffByIdAsync(Guid idStaff)
        {
            var staff = await _staffRepository.GetStaffByIdAsync(idStaff);
            if (staff == null)
            {
                throw new StaffNotFoundException();
            }
            return _mapper.Map<StaffResponse>(staff);
        }

        private void ValidateStaffCreation(StaffCreationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Fullname))
                throw new RequiredFieldException("Fullname");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RequiredFieldException("Email");

            if (!IsValidEmail(request.Email))
                throw new InvalidEmailException();

            if (string.IsNullOrWhiteSpace(request.Username))
                throw new RequiredFieldException("Username");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new RequiredFieldException("Password");

            if (request.Password.Length < 6)
                throw new InvalidPasswordException();

            if (!string.IsNullOrEmpty(request.Role) && request.Role != "ADMIN" && request.Role != "STAFF")
                throw new ValidationException("Role phải là ADMIN hoặc STAFF!");
        }

        private void ValidateStaffUpdate(StaffUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Fullname))
                throw new RequiredFieldException("Fullname");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RequiredFieldException("Email");

            if (!IsValidEmail(request.Email))
                throw new InvalidEmailException();

            if (!string.IsNullOrEmpty(request.Role) && request.Role != "ADMIN" && request.Role != "STAFF")
                throw new ValidationException("Role phải là ADMIN hoặc STAFF!");
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }
    }
}