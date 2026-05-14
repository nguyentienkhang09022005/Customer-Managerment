namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class DashboardResponse
    {
        public int TotalLeads { get; set; }
        public int TotalCustomers { get; set; }
        public int ActiveDeals { get; set; }
        public int TotalContacts { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal AverageDealValue { get; set; }
        public int LeadsCreatedThisMonth { get; set; }
        public int CustomersCreatedThisMonth { get; set; }
        public int DealsWonThisMonth { get; set; }
        public int DealsLostThisMonth { get; set; }
    }

    public class RevenueChartResponse
    {
        public List<RevenueDataPoint> DataPoints { get; set; } = new();
        public decimal TotalRevenue { get; set; }
        public decimal TotalLost { get; set; }
    }

    public class RevenueDataPoint
    {
        public DateTime Date { get; set; }
        public decimal WonAmount { get; set; }
        public decimal LostAmount { get; set; }
        public decimal PipelineValue { get; set; }
    }

    public class PipelineFunnelResponse
    {
        public int OpenDealsCount { get; set; }
        public int NegotiatingDealsCount { get; set; }
        public int WonDealsCount { get; set; }
        public decimal OpenDealsValue { get; set; }
        public decimal NegotiatingDealsValue { get; set; }
        public decimal WonDealsValue { get; set; }
    }

    public class StaffPerformanceResponse
    {
        public Guid IdStaff { get; set; }
        public string StaffName { get; set; } = null!;
        public int TotalDealsCreated { get; set; }
        public int WonDeals { get; set; }
        public int LostDeals { get; set; }
        public decimal WinRate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageDealValue { get; set; }
        public int ContactsCreated { get; set; }
        public int LeadsCreated { get; set; }
        public int TasksCompleted { get; set; }
    }

    public class LeadConversionResponse
    {
        public int TotalLeads { get; set; }
        public int ConvertedLeads { get; set; }
        public int LostLeads { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal AverageConversionDays { get; set; }
        public List<ConversionBySource> ConversionBySources { get; set; } = new();
    }

    public class ConversionBySource
    {
        public string Source { get; set; } = null!;
        public int Total { get; set; }
        public int Converted { get; set; }
        public decimal Rate { get; set; }
    }

    public class ExportReportResponse
    {
        public string FileName { get; set; } = null!;
        public string DownloadUrl { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public byte[] FileData { get; set; } = null!;
    }

    public class StaffPerformanceListResponse
    {
        public List<StaffPerformanceResponse> StaffPerformances { get; set; } = new();
        public int TotalStaff { get; set; }
    }
}