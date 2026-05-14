namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class AuditStatisticsResponse
    {
        public int TotalLogs { get; set; }
        public int CreateCount { get; set; }
        public int UpdateCount { get; set; }
        public int DeleteCount { get; set; }
        public int RestoreCount { get; set; }
        public Dictionary<string, int> EntityTypeCounts { get; set; } = new();
        public Dictionary<string, int> ActionCounts { get; set; } = new();
    }
}