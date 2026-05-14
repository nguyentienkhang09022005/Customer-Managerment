using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class CalendarHandler
    {
        private readonly ICalendarEventRepository _calendarRepository;
        private readonly IEventParticipantRepository _participantRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public CalendarHandler(
            ICalendarEventRepository calendarRepository,
            IEventParticipantRepository participantRepository,
            IStaffRepository staffRepository,
            INotificationRepository notificationRepository,
            IMapper mapper)
        {
            _calendarRepository = calendarRepository;
            _participantRepository = participantRepository;
            _staffRepository = staffRepository;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<CalendarEventResponse> CreateEventAsync(CalendarEventCreationRequest request)
        {
            ValidateCreation(request);

            var staff = await _staffRepository.GetStaffByIdAsync(request.IdStaff);
            if (staff == null)
            {
                throw new StaffNotFoundException();
            }

            var calendarEvent = new CalendarEvent
            {
                Title = request.Title,
                Description = request.Description,
                EventType = request.EventType,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Location = request.Location,
                IsAllDay = request.IsAllDay,
                ReminderMinutes = request.ReminderMinutes,
                Status = CalendarEventStatusConstant.EventStatusScheduled.ToStatusInt(),
                IdStaff = request.IdStaff,
                RelatedEntityType = request.RelatedEntityType,
                RelatedEntityId = request.RelatedEntityId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _calendarRepository.AddAsync(calendarEvent);
            var response = MapToResponse(created, staff);
            return response;
        }

        public async Task<CalendarEventResponse> UpdateEventAsync(CalendarEventUpdateRequest request, Guid idEvent)
        {
            var existingEvent = await _calendarRepository.GetByIdAsync(idEvent);
            if (existingEvent == null)
            {
                throw new CalendarEventNotFoundException();
            }

            ValidateUpdate(request);

            if (!string.IsNullOrEmpty(request.Title))
                existingEvent.Title = request.Title;
            if (request.Description != null)
                existingEvent.Description = request.Description;
            if (request.EventType.HasValue)
                existingEvent.EventType = request.EventType.Value;
            if (request.StartTime.HasValue)
                existingEvent.StartTime = request.StartTime.Value;
            if (request.EndTime.HasValue)
                existingEvent.EndTime = request.EndTime.Value;
            if (request.Location != null)
                existingEvent.Location = request.Location;
            if (request.IsAllDay.HasValue)
                existingEvent.IsAllDay = request.IsAllDay.Value;
            if (request.ReminderMinutes.HasValue)
                existingEvent.ReminderMinutes = request.ReminderMinutes.Value;
            if (request.Status.HasValue)
                existingEvent.Status = request.Status.Value;
            if (request.RelatedEntityType != null)
                existingEvent.RelatedEntityType = request.RelatedEntityType;
            if (request.RelatedEntityId.HasValue)
                existingEvent.RelatedEntityId = request.RelatedEntityId.Value;

            existingEvent.UpdatedAt = DateTime.UtcNow;

            var updated = await _calendarRepository.UpdateAsync(existingEvent);
            var staff = await _staffRepository.GetStaffByIdAsync(updated.IdStaff);
            var response = MapToResponse(updated, staff);

            var participants = await _participantRepository.GetByEventAsync(idEvent);
            response.Participants = await GetParticipantResponsesAsync(participants.ToList());

            return response;
        }

        public async Task<bool> DeleteEventAsync(Guid idEvent)
        {
            var result = await _calendarRepository.SoftDeleteAsync(idEvent);
            if (!result)
            {
                throw new CalendarEventNotFoundException();
            }
            return true;
        }

        public async Task<bool> CancelEventAsync(Guid idEvent)
        {
            var existingEvent = await _calendarRepository.GetByIdAsync(idEvent);
            if (existingEvent == null)
            {
                throw new CalendarEventNotFoundException();
            }

            existingEvent.Status = CalendarEventStatusConstant.EventStatusCancelled.ToStatusInt();
            existingEvent.UpdatedAt = DateTime.UtcNow;

            await _calendarRepository.UpdateAsync(existingEvent);
            return true;
        }

        public async Task<CalendarEventResponse> AddParticipantAsync(Guid idEvent, Guid idStaff)
        {
            var calendarEvent = await _calendarRepository.GetByIdAsync(idEvent);
            if (calendarEvent == null)
            {
                throw new CalendarEventNotFoundException();
            }

            var staff = await _staffRepository.GetStaffByIdAsync(idStaff);
            if (staff == null)
            {
                throw new StaffNotFoundException();
            }

            var existing = await _participantRepository.GetParticipantAsync(idEvent, idStaff);
            if (existing != null)
            {
                throw new ConflictException("Staff đã là người tham gia sự kiện này!");
            }

            var participant = new EventParticipant
            {
                IdEvent = idEvent,
                IdStaff = idStaff,
                Status = ParticipantStatusConstant.StatusPending.ToStatusInt()
            };

            var created = await _participantRepository.AddAsync(participant);

            var notification = new Notification
            {
                Title = "Bạn được thêm vào sự kiện mới",
                Message = $"Bạn được thêm vào sự kiện: {calendarEvent.Title}",
                Type = NotificationTypeConstant.NotificationSystem,
                IdStaff = idStaff,
                RelatedEntityType = "CalendarEvent",
                RelatedEntityId = idEvent
            };
            await _notificationRepository.AddNotificationAsync(notification);

            var response = MapToResponse(calendarEvent, staff);
            response.Participants = new List<EventParticipantResponse>
            {
                new EventParticipantResponse
                {
                    Id = created.Id,
                    IdEvent = created.IdEvent,
                    IdStaff = created.IdStaff,
                    Staff = MapStaffToResponse(staff),
                    Status = ParticipantStatusConstant.ToString(created.Status),
                    RespondedAt = created.RespondedAt
                }
            };

            return response;
        }

        public async Task<EventParticipantResponse> UpdateParticipantStatusAsync(Guid idParticipant, int status)
        {
            var participant = await _participantRepository.GetByIdAsync(idParticipant);
            if (participant == null)
            {
                throw new EventParticipantNotFoundException();
            }

            if (status < 0 || status > 3)
            {
                throw new InvalidStatusException(status.ToString());
            }

            participant.Status = status;
            participant.RespondedAt = DateTime.UtcNow;

            var updated = await _participantRepository.UpdateAsync(participant);
            var staff = await _staffRepository.GetStaffByIdAsync(updated.IdStaff);

            return new EventParticipantResponse
            {
                Id = updated.Id,
                IdEvent = updated.IdEvent,
                IdStaff = updated.IdStaff,
                Staff = staff != null ? MapStaffToResponse(staff) : null,
                Status = ParticipantStatusConstant.ToString(updated.Status),
                RespondedAt = updated.RespondedAt
            };
        }

        public async Task<bool> RemoveParticipantAsync(Guid idParticipant)
        {
            var result = await _participantRepository.RemoveAsync(idParticipant);
            if (!result)
            {
                throw new EventParticipantNotFoundException();
            }
            return true;
        }

        public async Task<CalendarEventResponse?> GetEventByIdAsync(Guid idEvent)
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

        public async Task<List<CalendarEventResponse>> GetEventsAsync(DateTime fromDate, DateTime toDate, Guid? idStaff = null)
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

        private void ValidateCreation(CalendarEventCreationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new RequiredFieldException("Title");

            if (request.Title.Length > 200)
                throw new InvalidLengthException("Title", 1, 200);

            if (request.EndTime < request.StartTime)
                throw new ValidationException("EndTime phải >= StartTime!");

            if (request.EventType < 0 || request.EventType > 3)
                throw new InvalidStatusException(request.EventType.ToString());
        }

        private void ValidateUpdate(CalendarEventUpdateRequest request)
        {
            if (!string.IsNullOrEmpty(request.Title) && request.Title.Length > 200)
                throw new InvalidLengthException("Title", 1, 200);

            if (request.StartTime.HasValue && request.EndTime.HasValue && request.EndTime < request.StartTime)
                throw new ValidationException("EndTime phải >= StartTime!");

            if (request.EventType.HasValue && (request.EventType < 0 || request.EventType > 3))
                throw new InvalidStatusException(request.EventType.Value.ToString());

            if (request.Status.HasValue && (request.Status < 0 || request.Status > 3))
                throw new InvalidStatusException(request.Status.Value.ToString());
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