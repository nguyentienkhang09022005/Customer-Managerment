using System.ComponentModel.DataAnnotations;

namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class CalendarEventInput
    {
        [Required(ErrorMessage = "Title là bắt buộc.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title phải từ 2-200 ký tự.")]
        public string Title { get; set; } = null!;

        [StringLength(2000, ErrorMessage = "Description tối đa 2000 ký tự.")]
        public string? Description { get; set; }

        public Enums.CalendarEventType EventType { get; set; }

        [Required(ErrorMessage = "StartTime là bắt buộc.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "EndTime là bắt buộc.")]
        public DateTime EndTime { get; set; }

        [StringLength(200, ErrorMessage = "Location tối đa 200 ký tự.")]
        public string? Location { get; set; }

        public bool IsAllDay { get; set; } = false;

        [Range(0, 10080, ErrorMessage = "ReminderMinutes phải từ 0 đến 10080 (7 ngày).")]
        public int? ReminderMinutes { get; set; }

        [Required(ErrorMessage = "IdStaff là bắt buộc.")]
        public Guid IdStaff { get; set; }

        [StringLength(50, ErrorMessage = "RelatedEntityType tối đa 50 ký tự.")]
        public string? RelatedEntityType { get; set; }

        public Guid? RelatedEntityId { get; set; }
    }

    public class CalendarEventUpdateInput
    {
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title phải từ 2-200 ký tự.")]
        public string? Title { get; set; }

        [StringLength(2000, ErrorMessage = "Description tối đa 2000 ký tự.")]
        public string? Description { get; set; }

        public Enums.CalendarEventType? EventType { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [StringLength(200, ErrorMessage = "Location tối đa 200 ký tự.")]
        public string? Location { get; set; }

        public bool? IsAllDay { get; set; }

        [Range(0, 10080, ErrorMessage = "ReminderMinutes phải từ 0 đến 10080 (7 ngày).")]
        public int? ReminderMinutes { get; set; }

        public Enums.CalendarEventStatus? Status { get; set; }

        [StringLength(50, ErrorMessage = "RelatedEntityType tối đa 50 ký tự.")]
        public string? RelatedEntityType { get; set; }

        public Guid? RelatedEntityId { get; set; }
    }

    public class EventParticipantInput
    {
        [Required(ErrorMessage = "IdEvent là bắt buộc.")]
        public Guid IdEvent { get; set; }

        [Required(ErrorMessage = "IdStaff là bắt buộc.")]
        public Guid IdStaff { get; set; }
    }
}