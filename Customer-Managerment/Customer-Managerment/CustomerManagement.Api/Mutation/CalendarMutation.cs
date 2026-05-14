using Customer_Managerment.CustomerManagement.Api.Input.Type;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class CalendarMutation
    {
        private readonly CalendarHandler _calendarHandler;

        public CalendarMutation(CalendarHandler calendarHandler)
        {
            _calendarHandler = calendarHandler;
        }

        public async Task<CalendarEventResponse> CreateCalendarEventAsync(CalendarEventInput input)
        {
            var request = new CalendarEventCreationRequest
            {
                Title = input.Title,
                Description = input.Description,
                EventType = (int)input.EventType,
                StartTime = input.StartTime,
                EndTime = input.EndTime,
                Location = input.Location,
                IsAllDay = input.IsAllDay,
                ReminderMinutes = input.ReminderMinutes,
                IdStaff = input.IdStaff,
                RelatedEntityType = input.RelatedEntityType,
                RelatedEntityId = input.RelatedEntityId
            };

            return await _calendarHandler.CreateEventAsync(request);
        }

        public async Task<CalendarEventResponse> UpdateCalendarEventAsync(CalendarEventUpdateInput input, Guid idEvent)
        {
            var request = new CalendarEventUpdateRequest
            {
                Title = input.Title,
                Description = input.Description,
                EventType = input.EventType.HasValue ? (int?)input.EventType.Value : null,
                StartTime = input.StartTime,
                EndTime = input.EndTime,
                Location = input.Location,
                IsAllDay = input.IsAllDay,
                ReminderMinutes = input.ReminderMinutes,
                Status = input.Status.HasValue ? (int?)input.Status.Value : null,
                RelatedEntityType = input.RelatedEntityType,
                RelatedEntityId = input.RelatedEntityId
            };

            return await _calendarHandler.UpdateEventAsync(request, idEvent);
        }

        public async Task<bool> DeleteCalendarEventAsync(Guid idEvent)
        {
            await _calendarHandler.DeleteEventAsync(idEvent);
            return true;
        }

        public async Task<bool> CancelCalendarEventAsync(Guid idEvent)
        {
            await _calendarHandler.CancelEventAsync(idEvent);
            return true;
        }

        public async Task<CalendarEventResponse> AddParticipantAsync(EventParticipantInput input)
        {
            return await _calendarHandler.AddParticipantAsync(input.IdEvent, input.IdStaff);
        }

        public async Task<EventParticipantResponse> UpdateParticipantStatusAsync(Guid idParticipant, int status)
        {
            return await _calendarHandler.UpdateParticipantStatusAsync(idParticipant, status);
        }

        public async Task<bool> RemoveParticipantAsync(Guid idParticipant)
        {
            await _calendarHandler.RemoveParticipantAsync(idParticipant);
            return true;
        }
    }
}