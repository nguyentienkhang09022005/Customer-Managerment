using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Application.UseCases;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class StaffPresenceQuery
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IStaffActivityLogRepository _activityLogRepository;
        private readonly IMapper _mapper;

        public StaffPresenceQuery(
            IStaffRepository staffRepository,
            IStaffActivityLogRepository activityLogRepository,
            IMapper mapper)
        {
            _staffRepository = staffRepository;
            _activityLogRepository = activityLogRepository;
            _mapper = mapper;
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<StaffStatusResponse> GetStaffStatuses()
        {
            var staffs = _staffRepository.GetListStaff().ToList();
            var responses = new List<StaffStatusResponse>();
            foreach (var staff in staffs)
            {
                responses.Add(new StaffStatusResponse
                {
                    IdStaff = staff.Id,
                    Fullname = staff.Fullname,
                    Email = staff.Email,
                    Status = staff.Status,
                    StatusName = Domain.Constant.StaffStatusConstant.ToString(staff.Status),
                    LastActiveAt = staff.LastActiveAt
                });
            }
            return responses.AsQueryable();
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<StaffStatusResponse> GetOnlineStaffs()
        {
            var staffs = _staffRepository.GetOnlineStaffsAsync().Result;
            var responses = new List<StaffStatusResponse>();
            foreach (var staff in staffs)
            {
                responses.Add(new StaffStatusResponse
                {
                    IdStaff = staff.Id,
                    Fullname = staff.Fullname,
                    Email = staff.Email,
                    Status = staff.Status,
                    StatusName = Domain.Constant.StaffStatusConstant.ToString(staff.Status),
                    LastActiveAt = staff.LastActiveAt
                });
            }
            return responses.AsQueryable();
        }

        public async Task<List<StaffActivityLogResponse>> GetStaffActivityLogsAsync(
            Guid idStaff,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var staff = await _staffRepository.GetStaffByIdAsync(idStaff);
            if (staff == null)
            {
                return new List<StaffActivityLogResponse>();
            }

            IQueryable<StaffActivityLog> logs;
            if (fromDate.HasValue && toDate.HasValue)
            {
                logs = await _activityLogRepository.GetLogsByStaffAndDateRangeAsync(idStaff, fromDate.Value, toDate.Value);
            }
            else
            {
                logs = await _activityLogRepository.GetLogsByStaffAsync(idStaff);
            }

            var responses = new List<StaffActivityLogResponse>();
            foreach (var log in logs.OrderByDescending(l => l.Timestamp).Take(100))
            {
                responses.Add(new StaffActivityLogResponse
                {
                    IdLog = log.IdLog,
                    IdStaff = log.IdStaff,
                    StaffName = staff.Fullname,
                    Action = log.Action,
                    EntityType = log.EntityType,
                    EntityId = log.EntityId,
                    Timestamp = log.Timestamp,
                    IpAddress = log.IpAddress,
                    UserAgent = log.UserAgent
                });
            }
            return responses;
        }
    }
}