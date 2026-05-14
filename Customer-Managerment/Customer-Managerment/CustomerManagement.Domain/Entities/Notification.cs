namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class Notification
    {
        public Guid IdNotification { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool IsRead { get; set; } = false;
        public bool IsPinned { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key - người nhận
        public Guid IdStaff { get; set; }

        // Related entity info
        public string? RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }

        // Navigation property
        public virtual Person? IdStaffNavigation { get; set; }
    }
}