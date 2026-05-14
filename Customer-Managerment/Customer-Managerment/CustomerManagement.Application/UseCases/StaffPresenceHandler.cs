using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class StaffPresenceHandler
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IStaffActivityLogRepository _activityLogRepository;
        private readonly IMapper _mapper;

        public StaffPresenceHandler(
            IStaffRepository staffRepository,
            IStaffActivityLogRepository activityLogRepository,
            IMapper mapper)
        {
            _staffRepository = staffRepository;
            _activityLogRepository = activityLogRepository;
            _mapper = mapper;
        }

        public async Task<StaffStatusResponse> UpdateMyStatusAsync(Guid idStaff, int status, string? ipAddress = null, string? userAgent = null)
        {
            var staff = await _staffRepository.GetStaffByIdAsync(idStaff);
            if (staff == null)
            {
                throw new StaffNotFoundException();
            }

            if (status < 0 || status > 3)
            {
                throw new InvalidStatusException(status.ToString());
            }

            if (status == 0)
            {
                throw new ValidationException("Staff không thể tự đặt OFFLINE. Vui lòng đăng xuất!");
            }

            await _staffRepository.UpdateStaffStatusAsync(idStaff, status);
            await _staffRepository.UpdateLastActiveAsync(idStaff);

            await LogActivityAsync(idStaff, status == 1 ? "LOGIN" : "STATUS_CHANGE", "Staff", idStaff, ipAddress, userAgent);

            staff.Status = status;
            staff.LastActiveAt = DateTime.UtcNow;
            return MapToStatusResponse(staff);
        }

        public async Task<StaffStatusResponse> RefreshLastActiveAsync(Guid idStaff, string? ipAddress = null, string? userAgent = null)
        {
            var staff = await _staffRepository.GetStaffByIdAsync(idStaff);
            if (staff == null)
            {
                throw new StaffNotFoundException();
            }

            await _staffRepository.UpdateLastActiveAsync(idStaff);

            staff.LastActiveAt = DateTime.UtcNow;
            return MapToStatusResponse(staff);
        }

        public async Task<List<StaffStatusResponse>> GetStaffStatusesAsync()
        {
            var staffs = await _staffRepository.GetListStaff().ToListAsync();
            return staffs.Select(MapToStatusResponse).ToList();
        }

        public async Task<List<StaffStatusResponse>> GetOnlineStaffsAsync()
        {
            var staffs = await _staffRepository.GetOnlineStaffsAsync();
            return staffs.Select(MapToStatusResponse).ToList();
        }

        public async Task<List<StaffActivityLogResponse>> GetActivityLogsAsync(Guid idStaff, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var staff = await _staffRepository.GetStaffByIdAsync(idStaff);
            if (staff == null)
            {
                throw new StaffNotFoundException();
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

            var logResponses = new List<StaffActivityLogResponse>();
            foreach (var log in logs.OrderByDescending(l => l.Timestamp).Take(100))
            {
                logResponses.Add(new StaffActivityLogResponse
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
            return logResponses;
        }

        public async Task LogActivityAsync(Guid idStaff, string action, string? entityType = null, Guid? entityId = null, string? ipAddress = null, string? userAgent = null)
        {
            var log = new StaffActivityLog
            {
                IdStaff = idStaff,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow
            };
            await _activityLogRepository.AddAsync(log);
        }

        private StaffStatusResponse MapToStatusResponse(Person staff)
        {
            return new StaffStatusResponse
            {
                IdStaff = staff.Id,
                Fullname = staff.Fullname,
                Email = staff.Email,
                Status = staff.Status,
                StatusName = StaffStatusConstant.ToString(staff.Status),
                LastActiveAt = staff.LastActiveAt
            };
        }
    }
}