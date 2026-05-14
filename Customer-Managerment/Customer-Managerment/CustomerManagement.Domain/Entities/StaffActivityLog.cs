namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class StaffActivityLog
    {
        public Guid IdLog { get; set; } = Guid.NewGuid();
        public Guid IdStaff { get; set; }
        public string Action { get; set; } = null!;
        public string? EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        public virtual Person? IdStaffNavigation { get; set; }
    }
}