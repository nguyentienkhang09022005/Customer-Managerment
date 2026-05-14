using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class CalendarQuery
    {
        private readonly ICalendarEventRepository _calendarRepository;
        private readonly IEventParticipantRepository _participantRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IMapper _mapper;

        public CalendarQuery(
            ICalendarEventRepository calendarRepository,
            IEventParticipantRepository participantRepository,
            IStaffRepository staffRepository,
            IMapper mapper)
        {
            _calendarRepository = calendarRepository;
            _participantRepository = participantRepository;
            _staffRepository = staffRepository;
            _mapper = mapper;
        }

        [UseFiltering]
        [UseSorting]
        public async Task<List<CalendarEventResponse>> GetCalendarEventsAsync(DateTime fromDate, DateTime toDate, Guid? idStaff = null)
        {
            IQueryable<CalendarEvent> events;
            if (idStaff.HasValue)
            {
                events = await _calendarRepository.GetByStaffAsync(idStaff.Value);
                events = events.Where(e => e.StartTime >= fromDate && e.EndTime <= toDate);
            }
            else
            {
                events = await _calendarRepository.GetByDateRangeAsync(fromDate, toDate);
            }

            var eventList = events.OrderBy(e => e.StartTime).ToList();
            var responses = new List<CalendarEventResponse>();

            foreach (var evt in eventList)
            {
                var staff = await _staffRepository.GetStaffByIdAsync(evt.IdStaff);
                var response = MapToResponse(evt, staff);
                var participants = await _participantRepository.GetByEventAsync(evt.IdEvent);
                response.Participants = await GetParticipantResponsesAsync(participants.ToList());
                responses.Add(response);
            }

            return responses;
        }

        public async Task<CalendarEventResponse?> GetCalendarEventByIdAsync(Guid idEvent)
        {
            var calendarEvent = await _calendarRepository.GetByIdAsync(idEvent);
            if (calendarEvent == null)
                return null;

            var staff = await _staffRepository.GetStaffByIdAsync(calendarEvent.IdStaff);
            var response = MapToResponse(calendarEvent, staff);

            var participants = await _participantRepository.GetByEventAsync(idEvent);
            response.Participants = await GetParticipantResponsesAsync(participants.ToList());

            return response;
        }

        [UseFiltering]
        [UseSorting]
        public async Task<List<CalendarEventResponse>> GetMyEventsAsync(Guid idStaff, DateTime fromDate, DateTime toDate)
        {
            var myEvents = await _calendarRepository.GetByStaffAsync(idStaff);
            var events = myEvents.Where(e => e.StartTime >= fromDate && e.EndTime <= toDate)
                .OrderBy(e => e.StartTime)
                .ToList();

            var responses = new List<CalendarEventResponse>();
            foreach (var evt in events)
            {
                var staff = await _staffRepository.GetStaffByIdAsync(evt.IdStaff);
                var response = MapToResponse(evt, staff);
                var participants = await _participantRepository.GetByEventAsync(evt.IdEvent);
                response.Participants = await GetParticipantResponsesAsync(participants.ToList());
                responses.Add(response);
            }

            return responses;
        }

        public async Task<List<CalendarEventResponse>> GetUpcomingEventsAsync(Guid idStaff, int days)
        {
            var events = await _calendarRepository.GetUpcomingEventsAsync(idStaff, days);
            var eventList = events.OrderBy(e => e.StartTime).Take(20).ToList();

            var responses = new List<CalendarEventResponse>();
            foreach (var evt in eventList)
            {
                var staff = await _staffRepository.GetStaffByIdAsync(evt.IdStaff);
                var response = MapToResponse(evt, staff);
                var participants = await _participantRepository.GetByEventAsync(evt.IdEvent);
                response.Participants = await GetParticipantResponsesAsync(participants.ToList());
                responses.Add(response);
            }

            return responses;
        }

        public async Task<List<EventParticipantResponse>> GetEventParticipantsAsync(Guid idEvent)
        {
            var participants = await _participantRepository.GetByEventAsync(idEvent);
            return await GetParticipantResponsesAsync(participants.ToList());
        }

        private CalendarEventResponse MapToResponse(CalendarEvent evt, Person? staff)
        {
            return new CalendarEventResponse
            {
                IdEvent = evt.IdEvent,
                Title = evt.Title,
                Description = evt.Description,
                EventType = CalendarEventTypeConstant.ToString(evt.EventType),
                StartTime = evt.StartTime,
                EndTime = evt.EndTime,
                Location = evt.Location,
                IsAllDay = evt.IsAllDay,
                ReminderMinutes = evt.ReminderMinutes,
                Status = CalendarEventStatusConstant.ToString(evt.Status),
                CreatedAt = evt.CreatedAt,
                UpdatedAt = evt.UpdatedAt,
                IdStaff = evt.IdStaff,
                Staff = staff != null ? MapStaffToResponse(staff) : null,
                RelatedEntityType = evt.RelatedEntityType,
                RelatedEntityId = evt.RelatedEntityId
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

        private async Task<List<EventParticipantResponse>> GetParticipantResponsesAsync(List<EventParticipant> participants)
        {
            var responses = new List<EventParticipantResponse>();
            foreach (var p in participants)
            {
                var staff = await _staffRepository.GetStaffByIdAsync(p.IdStaff);
                responses.Add(new EventParticipantResponse
                {
                    Id = p.Id,
                    IdEvent = p.IdEvent,
                    IdStaff = p.IdStaff,
                    Staff = staff != null ? MapStaffToResponse(staff) : null,
                    Status = ParticipantStatusConstant.ToString(p.Status),
                    RespondedAt = p.RespondedAt
                });
            }
            return responses;
        }
    }
}