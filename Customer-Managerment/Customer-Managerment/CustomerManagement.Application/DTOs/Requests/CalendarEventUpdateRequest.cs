namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class CalendarEventUpdateRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? EventType { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Location { get; set; }
        public bool? IsAllDay { get; set; }
        public int? ReminderMinutes { get; set; }
        public int? Status { get; set; }
        public string? RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }
    }
}