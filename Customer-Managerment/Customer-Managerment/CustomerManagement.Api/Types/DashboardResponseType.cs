using Customer_Managerment.CustomerManagement.Application.DTOs.Response;

namespace Customer_Managerment.CustomerManagement.Api.Types
{
    public class DashboardResponseType : ObjectType<DashboardResponse>
    {
        protected override void Configure(IObjectTypeDescriptor<DashboardResponse> descriptor)
        {
            descriptor.Name("DashboardResponse");

            descriptor.Field(d => d.TotalLeads);
            descriptor.Field(d => d.TotalCustomers);
            descriptor.Field(d => d.ActiveDeals);
            descriptor.Field(d => d.TotalContacts);
            descriptor.Field(d => d.TotalRevenue);
            descriptor.Field(d => d.ConversionRate);
            descriptor.Field(d => d.AverageDealValue);
            descriptor.Field(d => d.LeadsCreatedThisMonth);
            descriptor.Field(d => d.CustomersCreatedThisMonth);
            descriptor.Field(d => d.DealsWonThisMonth);
            descriptor.Field(d => d.DealsLostThisMonth);
        }
    }

    public class RevenueChartResponseType : ObjectType<RevenueChartResponse>
    {
        protected override void Configure(IObjectTypeDescriptor<RevenueChartResponse> descriptor)
        {
            descriptor.Name("RevenueChartResponse");

            descriptor.Field(r => r.DataPoints);
            descriptor.Field(r => r.TotalRevenue);
            descriptor.Field(r => r.TotalLost);
        }
    }

    public class RevenueDataPointType : ObjectType<RevenueDataPoint>
    {
        protected override void Configure(IObjectTypeDescriptor<RevenueDataPoint> descriptor)
        {
            descriptor.Name("RevenueDataPoint");

            descriptor.Field(r => r.Date);
            descriptor.Field(r => r.WonAmount);
            descriptor.Field(r => r.LostAmount);
            descriptor.Field(r => r.PipelineValue);
        }
    }

    public class PipelineFunnelResponseType : ObjectType<PipelineFunnelResponse>
    {
        protected override void Configure(IObjectTypeDescriptor<PipelineFunnelResponse> descriptor)
        {
            descriptor.Name("PipelineFunnelResponse");

            descriptor.Field(p => p.OpenDealsCount);
            descriptor.Field(p => p.NegotiatingDealsCount);
            descriptor.Field(p => p.WonDealsCount);
            descriptor.Field(p => p.OpenDealsValue);
            descriptor.Field(p => p.NegotiatingDealsValue);
            descriptor.Field(p => p.WonDealsValue);
        }
    }

    public class StaffPerformanceResponseType : ObjectType<StaffPerformanceResponse>
    {
        protected override void Configure(IObjectTypeDescriptor<StaffPerformanceResponse> descriptor)
        {
            descriptor.Name("StaffPerformanceResponse");

            descriptor.Field(s => s.IdStaff);
            descriptor.Field(s => s.StaffName);
            descriptor.Field(s => s.TotalDealsCreated);
            descriptor.Field(s => s.WonDeals);
            descriptor.Field(s => s.LostDeals);
            descriptor.Field(s => s.WinRate);
            descriptor.Field(s => s.TotalRevenue);
            descriptor.Field(s => s.AverageDealValue);
            descriptor.Field(s => s.ContactsCreated);
            descriptor.Field(s => s.LeadsCreated);
            descriptor.Field(s => s.TasksCompleted);
        }
    }

    public class StaffPerformanceListResponseType : ObjectType<StaffPerformanceListResponse>
    {
        protected override void Configure(IObjectTypeDescriptor<StaffPerformanceListResponse> descriptor)
        {
            descriptor.Name("StaffPerformanceListResponse");

            descriptor.Field(s => s.StaffPerformances);
            descriptor.Field(s => s.TotalStaff);
        }
    }

    public class LeadConversionResponseType : ObjectType<LeadConversionResponse>
    {
        protected override void Configure(IObjectTypeDescriptor<LeadConversionResponse> descriptor)
        {
            descriptor.Name("LeadConversionResponse");

            descriptor.Field(l => l.TotalLeads);
            descriptor.Field(l => l.ConvertedLeads);
            descriptor.Field(l => l.LostLeads);
            descriptor.Field(l => l.ConversionRate);
            descriptor.Field(l => l.AverageConversionDays);
            descriptor.Field(l => l.ConversionBySources);
        }
    }

    public class ConversionBySourceType : ObjectType<ConversionBySource>
    {
        protected override void Configure(IObjectTypeDescriptor<ConversionBySource> descriptor)
        {
            descriptor.Name("ConversionBySource");

            descriptor.Field(c => c.Source);
            descriptor.Field(c => c.Total);
            descriptor.Field(c => c.Converted);
            descriptor.Field(c => c.Rate);
        }
    }

    public class ExportReportResponseType : ObjectType<ExportReportResponse>
    {
        protected override void Configure(IObjectTypeDescriptor<ExportReportResponse> descriptor)
        {
            descriptor.Name("ExportReportResponse");

            descriptor.Field(e => e.FileName);
            descriptor.Field(e => e.DownloadUrl);
            descriptor.Field(e => e.ContentType);
        }
    }
}