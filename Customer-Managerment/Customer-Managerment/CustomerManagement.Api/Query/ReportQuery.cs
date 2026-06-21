using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Microsoft.AspNetCore.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    [Authorize(Roles = "ADMIN")]
    public class ReportQuery
    {
        private readonly ReportHandler _reportHandler;
        private readonly ExportHandler _exportHandler;
        private readonly IMapper _mapper;

        public ReportQuery(ReportHandler reportHandler, ExportHandler exportHandler, IMapper mapper)
        {
            _reportHandler = reportHandler;
            _exportHandler = exportHandler;
            _mapper = mapper;
        }

        public async Task<DashboardResponse> GetDashboardSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            return await _reportHandler.GetDashboardSummaryAsync(fromDate, toDate);
        }

        public async Task<RevenueChartResponse> GetRevenueChartAsync(DateTime fromDate, DateTime toDate, string groupBy = "day")
        {
            return await _reportHandler.GetRevenueChartAsync(fromDate, toDate, groupBy);
        }

        public async Task<PipelineFunnelResponse> GetPipelineFunnelAsync()
        {
            return await _reportHandler.GetPipelineFunnelAsync();
        }

        public async Task<StaffPerformanceListResponse> GetTopPerformingStaffAsync(int limit = 10)
        {
            return await _reportHandler.GetTopPerformingStaffAsync(limit);
        }

        public async Task<StaffPerformanceResponse> GetStaffPerformanceReportAsync(Guid idStaff, DateTime fromDate, DateTime toDate)
        {
            return await _reportHandler.GetStaffPerformanceAsync(idStaff, fromDate, toDate);
        }

        public async Task<LeadConversionResponse> GetLeadConversionReportAsync(DateTime fromDate, DateTime toDate)
        {
            return await _reportHandler.GetLeadConversionReportAsync(fromDate, toDate);
        }

        public async Task<ExportReportResponse> ExportDealsReportAsync(DateTime fromDate, DateTime toDate)
        {
            return await _exportHandler.ExportDealsToExcelAsync(fromDate, toDate);
        }

        public async Task<ExportReportResponse> ExportLeadsReportAsync(DateTime fromDate, DateTime toDate)
        {
            return await _exportHandler.ExportLeadsToExcelAsync(fromDate, toDate);
        }

        public async Task<ExportReportResponse> ExportCustomersReportAsync(DateTime fromDate, DateTime toDate)
        {
            return await _exportHandler.ExportCustomersToExcelAsync(fromDate, toDate);
        }
    }
}