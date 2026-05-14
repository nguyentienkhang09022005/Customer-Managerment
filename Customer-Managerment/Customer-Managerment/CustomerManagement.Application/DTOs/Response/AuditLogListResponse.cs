namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class AuditLogListResponse
    {
        public List<AuditLogResponse> Logs { get; set; } = new();
        public int TotalCount { get; set; }
    }
}