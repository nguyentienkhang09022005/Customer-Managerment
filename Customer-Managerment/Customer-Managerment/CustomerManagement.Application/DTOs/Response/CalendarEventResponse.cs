namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class CalendarEventResponse
    {
        public Guid IdEvent { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string EventType { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Location { get; set; }
        public bool IsAllDay { get; set; }
        public int? ReminderMinutes { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid IdStaff { get; set; }
        public StaffResponse? Staff { get; set; }
        public string? RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public List<EventParticipantResponse>? Participants { get; set; }
    }
}