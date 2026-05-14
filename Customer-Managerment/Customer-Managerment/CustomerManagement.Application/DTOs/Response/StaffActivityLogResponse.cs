namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class StaffActivityLogResponse
    {
        public Guid IdLog { get; set; }
        public Guid IdStaff { get; set; }
        public string StaffName { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string? EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}