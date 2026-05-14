namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class CalendarEventInput
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public Enums.CalendarEventType EventType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Location { get; set; }
        public bool IsAllDay { get; set; } = false;
        public int? ReminderMinutes { get; set; }
        public Guid IdStaff { get; set; }
        public string? RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }
    }

    public class CalendarEventUpdateInput
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Enums.CalendarEventType? EventType { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Location { get; set; }
        public bool? IsAllDay { get; set; }
        public int? ReminderMinutes { get; set; }
        public Enums.CalendarEventStatus? Status { get; set; }
        public string? RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }
    }

    public class EventParticipantInput
    {
        public Guid IdEvent { get; set; }
        public Guid IdStaff { get; set; }
    }
}