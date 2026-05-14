namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class AuditLogResponse
    {
        public Guid IdLog { get; set; }
        public string Action { get; set; } = null!;
        public string EntityType { get; set; } = null!;
        public Guid EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public Guid? IdStaff { get; set; }
        public string? StaffName { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Description { get; set; }
    }
}