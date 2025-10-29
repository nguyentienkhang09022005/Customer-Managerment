using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class StaffQuery
    {
        private readonly IStaffRepository _staffRepository;

        public StaffQuery(IStaffRepository staffRepository) 
        {
            _staffRepository = staffRepository;
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<StaffResponse> GetStaffs()
        {
            return _staffRepository.GetListStaff();
        }

        public IQueryable<StaffResponse> GetStaffById(Guid idStaff)
        {
            return _staffRepository.GetStaffById(idStaff);
        }
    }
}
