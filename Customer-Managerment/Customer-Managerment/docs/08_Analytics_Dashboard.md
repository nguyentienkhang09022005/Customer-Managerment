# NHÓM 8: BÁO CÁO & THỐNG KÊ (Analytics Dashboard)

## Mục tiêu
Dashboard tổng quan cho Admin, biểu đồ doanh thu/pipeline, báo cáo hiệu suất nhân viên và export Excel/PDF.

---

## 1. Database Schema

### Không cần thêm bảng mới
Sử dụng dữ liệu từ các entities hiện có + aggregate queries.

### Views/Cached Data (tùy chọn cho performance)
Tạo materialized views hoặc cache table cho dashboard.

---

## 2. Files cần tạo mới

### Domain Layer
- `Domain/Constant/ReportPeriodConstant.cs`
- `Domain/Exception/ReportGenerationException.cs`

### Application Layer

**Request DTOs:**
- `DashboardRequest.cs` -- period, dateFrom, dateTo
- `StaffPerformanceRequest.cs` -- staffId, period

**Response DTOs:**
- `DashboardResponse.cs` -- Tổng quan
- `RevenueChartResponse.cs` -- Biểu đồ doanh thu
- `PipelineFunnelResponse.cs` -- Pipeline funnel
- `StaffPerformanceResponse.cs` -- Hiệu suất nhân viên
- `LeadConversionResponse.cs` -- Tỷ lệ chuyển đổi
- `ExportReportResponse.cs` -- Download URL

### UseCases
- `ReportHandler.cs` -- Tạo các loại báo cáo
- `DashboardHandler.cs` -- Dashboard data
- `ExportHandler.cs` -- Export Excel/PDF

### Infrastructure Layer
- `Services/ExcelExportService.cs` -- Export Excel
- `Services/PdfExportService.cs` -- Export PDF
- `Services/ChartDataService.cs` -- Chuẩn bị data cho chart

---

## 3. API Endpoints

### Dashboard Queries
- `getDashboardSummary(period)` - Tổng quan (Leads, Customers, Deals, Contacts count)
- `getRevenueChart(fromDate, toDate, groupBy)` - Biểu đồ doanh thu (theo ngày/tháng)
- `getPipelineFunnel()` - Pipeline funnel (OPEN → NEGOTIATING → WON)
- `getTopPerformingStaff(limit)` - Staff có deal won cao nhất

### Report Queries
- `getStaffPerformanceReport(staffId, fromDate, toDate)` - Báo cáo cá nhân
- `getLeadConversionReport(fromDate, toDate)` - Tỷ lệ chuyển đổi Lead→Customer
- `getContactStatisticsReport(fromDate, toDate)` - Thống kê contacts
- `getDealWinLossReport(fromDate, toDate)` - Deal thắng/thua

### Export
- `exportReportToExcel(reportType, params)` - Export Excel
- `exportReportToPdf(reportType, params)` - Export PDF

---

## 4. Dashboard Metrics

### Summary Cards
```
- Total Revenue (sum of WON deals)
- Total Leads (count)
- Total Customers (count)
- Active Deals (count)
- Conversion Rate (Leads → Customers %)
- Average Deal Value
```

### Revenue Chart
```
X-axis: Time (day/month)
Y-axis: Revenue amount
Series:
  - Won Deals (green)
  - Lost Deals (red)
  - Pipeline Value (blue)
```

### Pipeline Funnel
```
Stage 1: OPEN - Tổng số deals
Stage 2: NEGOTIATING - Deals đang đàm phán
Stage 3: WON - Deals thắng
(Lost - không trong funnel)
```

---

## 5. Staff Performance Metrics

| Metric | Description |
|--------|-------------|
| Total Deals Created | Số deal đã tạo |
| Won Deals | Số deal thắng |
| Lost Deals | Số deal thua |
| Win Rate | Won / Total Deals |
| Total Revenue | Sum của WON deals |
| Avg Deal Value | Total Revenue / Won Deals |
| Contacts Created | Số contacts đã tạo |
| Leads Created | Số leads đã tạo |
| Tasks Completed | Số tasks hoàn thành |

---

## 6. Report Types

| Report | Content | Export |
|--------|---------|--------|
| Revenue Report | Doanh thu theo thời gian | Excel, PDF |
| Lead Report | Danh sách + trạng thái leads | Excel |
| Customer Report | Danh sách customers | Excel |
| Deal Report | Deals + status + giá trị | Excel |
| Staff Activity Report | Hoạt động của staff | Excel, PDF |
| Conversion Report | Lead → Customer funnel | Excel |

---

## 7. Export Implementation

### Excel Export (EPPlus - đã dùng trong LeadHandler)
```csharp
using OfficeOpenXml;
// Use existing ExcelPackage pattern
var package = new ExcelPackage();
var worksheet = package.Workbook.Worksheets.Add(reportName);
// Add headers, data, styling
// Return as byte[] or file stream
```

### PDF Export
- Dùng QuestPDF hoặc iTextSharp
- Tạo layout với header, table, chart image

---

## 8. Business Rules

### Date Range
- Default: last 30 days
- Max range: 1 year
- Required: fromDate, toDate

### Permissions
- Chỉ ADMIN mới xem được reports
- Staff chỉ xem được performance report của mình

### Caching
- Dashboard summary: cache 5 minutes
- Charts: cache 10 minutes
- Report export: generate fresh each time

---

## 9. Integration

### Với StatisticsHandler (hiện có)
- Mở rộng QuantityStatisticsResponse thành DashboardResponse
- Thêm new metrics

### Với ChartDealHandler (hiện có)
- Sử dụng làm base cho revenue chart
- Mở rộng thêm filter by date range

---

## 10. Implementation Order

1. Handler + Response DTOs
2. ExcelExportService
3. PDF Export Service (tùy chọn)
4. GraphQL Query types
5. Integration với existing statistics
6. Cập nhật StatisticsHandler
7. Đăng ký services