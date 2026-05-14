namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class CalendarEventCreationRequest
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int EventType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Location { get; set; }
        public bool IsAllDay { get; set; } = false;
        public int? ReminderMinutes { get; set; }
        public Guid IdStaff { get; set; }
        public string? RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }
    }
}