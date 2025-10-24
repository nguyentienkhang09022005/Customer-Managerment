using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class StaffMutation
    {
        private readonly StaffHandler _staffHandler;

        public StaffMutation(StaffHandler staffHandler)
        {
            _staffHandler = staffHandler;
        }

        public async Task<StaffResponse> CreateStaffAsync(StaffCreationRequest staffCreationRequest)
        {
            return await _staffHandler.CreateStaffAsync(staffCreationRequest);
        }

        public async Task<string> DeleteStaffAsync(Guid idStaff)
        {
            return await _staffHandler.DeleteStaffAsync(idStaff);
        }

        public async Task<StaffResponse> UpdateStaffAsync(StaffUpdateRequest staffUpdateRequest, Guid idStaff)
        {
            return await _staffHandler.UpdateStaffAsync(staffUpdateRequest, idStaff);
        }
    }
}
