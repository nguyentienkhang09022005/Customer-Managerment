using Customer_Managerment.CustomerManagement.Application.DTOs.Response;

namespace Customer_Managerment.CustomerManagement.Api.Types
{
    public class CalendarEventResponseType : ObjectType<CalendarEventResponse>
    {
        protected override void Configure(IObjectTypeDescriptor<CalendarEventResponse> descriptor)
        {
            descriptor.Name("CalendarEventResponse");

            descriptor.Field(e => e.IdEvent);
            descriptor.Field(e => e.Title);
            descriptor.Field(e => e.Description);
            descriptor.Field(e => e.EventType);
            descriptor.Field(e => e.StartTime);
            descriptor.Field(e => e.EndTime);
            descriptor.Field(e => e.Location);
            descriptor.Field(e => e.IsAllDay);
            descriptor.Field(e => e.ReminderMinutes);
            descriptor.Field(e => e.Status);
            descriptor.Field(e => e.CreatedAt);
            descriptor.Field(e => e.UpdatedAt);
            descriptor.Field(e => e.IdStaff);
            descriptor.Field(e => e.Staff);
            descriptor.Field(e => e.RelatedEntityType);
            descriptor.Field(e => e.RelatedEntityId);
            descriptor.Field(e => e.Participants).Type<ListType<EventParticipantResponseType>>();
        }
    }
}