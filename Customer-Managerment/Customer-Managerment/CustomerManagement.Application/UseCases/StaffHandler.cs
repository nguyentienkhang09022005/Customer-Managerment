using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class StaffHandler
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IMapper _mapper;

        public StaffHandler(IStaffRepository staffRepository, IMapper mapper)
        {
            _staffRepository = staffRepository;
            _mapper = mapper;
        }

        public async Task<StaffResponse> CreateStaffAsync(StaffCreationRequest staffCreationRequest)
        {
            var checkEmail = await _staffRepository.GetStaffByEmailAsync(staffCreationRequest.Email);
            if (checkEmail != null){
                throw new DomainException("Email đã tồn tại!", 409);
            }

            var staffDomain = _mapper.Map<StaffDomain>(staffCreationRequest);

            var createdStaff = await _staffRepository.AddStaffAsync(staffDomain);
            return _mapper.Map<StaffResponse>(createdStaff);
        }

        public async Task<string> DeleteStaffAsync(Guid idStaff)
        {
            await _staffRepository.DeleteStaffAsync(idStaff);

            return "Xóa nhân viên thành công!";
        }

        public async Task<StaffResponse> UpdateStaffAsync(StaffUpdateRequest staffUpdateRequest, Guid idStaff)
        {
            var checkEmail = await _staffRepository.GetStaffByEmailAsync(staffUpdateRequest.Email);
            if (checkEmail != null){
                throw new DomainException("Email đã tồn tại!", 409);
            }

            var existingStaff = await _staffRepository.GetStaffByIdAsync(idStaff);
            if (existingStaff == null)
            {
                throw new DomainException("Nhân viên không tồn tại!", 404);
            }
            _mapper.Map(staffUpdateRequest, existingStaff);

            var updatedStaff = await _staffRepository.UpdateStaffAsync(existingStaff);
            return _mapper.Map<StaffResponse>(updatedStaff);
        }

        public async Task<StaffResponse> GetStaffByIdAsync(Guid idStaff)
        {
            var staffDomain = await _staffRepository.GetStaffByIdAsync(idStaff);
            if (staffDomain == null)
            {
                throw new DomainException("Nhân viên không tồn tại!", 404);
            }
            return _mapper.Map<StaffResponse>(staffDomain);
        }
    }
}
