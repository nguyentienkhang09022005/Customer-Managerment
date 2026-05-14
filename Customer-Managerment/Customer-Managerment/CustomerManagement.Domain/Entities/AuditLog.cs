namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class AuditLog
    {
        public Guid IdLog { get; set; } = Guid.NewGuid();
        public string Action { get; set; } = null!;
        public string EntityType { get; set; } = null!;
        public Guid EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public Guid? IdStaff { get; set; }
        public string? StaffName { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Description { get; set; }

        public virtual Person? IdStaffNavigation { get; set; }
    }
}