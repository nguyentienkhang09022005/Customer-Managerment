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

        // Computed fields
        public int totalActions => ActionCounts.Count;
        public int totalEntities => EntityTypeCounts.Count;
        public List<TopActionItem> topActions => ActionCounts
            .OrderByDescending(x => x.Value)
            .Take(5)
            .Select(x => new TopActionItem { action = x.Key, count = x.Value })
            .ToList();
        public List<TopEntityItem> topEntities => EntityTypeCounts
            .OrderByDescending(x => x.Value)
            .Take(5)
            .Select(x => new TopEntityItem { entityType = x.Key, count = x.Value })
            .ToList();
    }

    public class TopActionItem
    {
        public string action { get; set; } = "";
        public int count { get; set; }
    }

    public class TopEntityItem
    {
        public string entityType { get; set; } = "";
        public int count { get; set; }
    }
}