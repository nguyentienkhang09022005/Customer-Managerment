using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class StaffQuery
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IMapper _mapper;

        public StaffQuery(IStaffRepository staffRepository, IMapper mapper)
        {
            _staffRepository = staffRepository;
            _mapper = mapper;
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<StaffResponse> GetStaffs()
        {
            var staffs = _staffRepository.GetListStaff();
            return staffs.ProjectTo<StaffResponse>(_mapper.ConfigurationProvider);
        }

        public StaffResponse? GetStaffById(Guid idStaff)
        {
            var staff = _staffRepository.GetStaffById(idStaff).FirstOrDefault();
            return staff == null ? null : _mapper.Map<StaffResponse>(staff);
        }
    }
}