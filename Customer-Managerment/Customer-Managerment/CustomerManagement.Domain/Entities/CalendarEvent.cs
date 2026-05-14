namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class CalendarEvent
    {
        public Guid IdEvent { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int EventType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Location { get; set; }
        public bool IsAllDay { get; set; } = false;
        public int? ReminderMinutes { get; set; }
        public int Status { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public Guid IdStaff { get; set; }
        public string? RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }

        public virtual Person? IdStaffNavigation { get; set; }
        public virtual ICollection<EventParticipant> Participants { get; set; } = new List<EventParticipant>();
    }
}