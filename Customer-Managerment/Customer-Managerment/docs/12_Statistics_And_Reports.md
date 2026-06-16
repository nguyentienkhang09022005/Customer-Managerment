# NHÓM 12: THỐNG KÊ & BÁO CÁO (Statistics & Reports)

## Mục tiêu
Cung cấp dữ liệu tổng hợp cho dashboard quản trị: thống kê số lượng, doanh thu, tỉ lệ chuyển đổi, hiệu suất nhân viên, pipeline funnel, biểu đồ doanh thu theo thời gian. Đồng thời xuất báo cáo Excel.

---

## 1. Cấu trúc file

### Application Layer
- `Application/UseCases/StatisticsHandler.cs` — số liệu tổng quan (Quantity Statistics + Chart Deal).
- `Application/UseCases/ChartDealHandler.cs` — chart deal thắng/thua.
- `Application/UseCases/ReportHandler.cs` — dashboard, revenue chart, pipeline funnel, staff performance, lead conversion.
- `Application/UseCases/ExportHandler.cs` — xuất Excel (Deal, Lead, Customer).

### API Layer
- `Api/Query/StatisticsQuery.cs` — `getStatistics`, `getChartDeal`.
- `Api/Query/ReportQuery.cs` — các report + export.

---

## 2. GraphQL Endpoints

### Statistics
- `getStatistics(): QuantityStatisticsResponse!`
  - `TotalProfit`, `QuantityLeads`, `QuantityCustomers`, `QuantityContacts`, `QuantityDeals`.
  - `quantityStatisticsDetailContactResponse` (chi tiết contacts theo status).
  - `quantityStatisticsDetailDealResponse` (chi tiết deals theo status).
- `getChartDeal(): ChartDealResponse!`
  - `SuccessfullDealValue` (tổng Price của deal WON).
  - `FailedDealValue` (tổng Price của deal LOST).
  - `ListSuccessfullDeal`, `ListFailedDeal` (danh sách chi tiết).

### Reports
- `getDashboardSummary(fromDate, toDate): DashboardResponse!`
- `getRevenueChart(fromDate, toDate, groupBy="day"|"month"): RevenueChartResponse!`
- `getPipelineFunnel(): PipelineFunnelResponse!`
- `getTopPerformingStaff(limit=10): StaffPerformanceListResponse!`
- `getStaffPerformanceReport(idStaff, fromDate, toDate): StaffPerformanceResponse!`
- `getLeadConversionReport(fromDate, toDate): LeadConversionResponse!`
- `exportDealsReport(fromDate, toDate): ExportReportResponse!`
- `exportLeadsReport(fromDate, toDate): ExportReportResponse!`
- `exportCustomersReport(fromDate, toDate): ExportReportResponse!`

---

## 3. Luồng nghiệp vụ

### 3.1. Quantity Statistics
`StatisticsHandler.GetQuantityStatisticsResponseAsync`:
1. Tính `TotalProfit = sum(Price) của deal có Status = WON`.
2. Đếm tổng `Leads`, `Customers`, `Contacts`, `Deals`.
3. Lấy detail: `QuantityStatisticsDetailContactResponse` (theo status NEW/IN_PROGRESS/...), `QuantityStatisticsDetailDealResponse` (theo status OPEN/NEGOTIATING/WON/LOST).
4. Trả về tổng hợp.

### 3.2. Chart Deal
`ChartDealHandler.ChartDealResponseAsync`:
1. Lấy toàn bộ deal.
2. Tính `SuccessfullDealValue = sum(Price)` deal WON, `FailedDealValue = sum(Price)` deal LOST.
3. Map list deal thắng/thua sang response.
4. Trả về `ChartDealResponse`.

### 3.3. Dashboard Summary
`ReportHandler.GetDashboardSummaryAsync(fromDate, toDate)`:
1. Đếm:
   - `TotalLeads`, `TotalCustomers`, `ActiveDeals` (OPEN + NEGOTIATING), `TotalContacts`.
   - `LeadsCreatedThisMonth`, `CustomersCreatedThisMonth`.
   - `DealsWonThisMonth`, `DealsLostThisMonth`.
2. `TotalRevenue = sum(Price)` deal WON (toàn thời gian, không filter date).
3. `AverageDealValue = TotalRevenue / WonDeals.Count` (hoặc 0).
4. `ConversionRate = (Customers.Count / Leads.Count) * 100` (toàn thời gian).
5. Trả về `DashboardResponse`.

### 3.4. Revenue Chart
`ReportHandler.GetRevenueChartAsync(fromDate, toDate, groupBy)`:
- Nếu `groupBy = "day"`:
  - Tạo list các ngày từ fromDate đến toDate.
  - Với mỗi ngày: `WonAmount`, `LostAmount`, `PipelineValue` (OPEN + NEGOTIATING) của deal `CreatedAt.Date == day`.
- Nếu `groupBy = "month"` (hoặc khác "day"):
  - Tạo 13 tháng liên tiếp (từ fromDate.Year/Month).
  - Với mỗi tháng: aggregate tương tự.
- Trả về `RevenueChartResponse { DataPoints, TotalRevenue, TotalLost }`.

### 3.5. Pipeline Funnel
- Đếm `OpenDealsCount`, `NegotiatingDealsCount`, `WonDealsCount`.
- Tính `OpenDealsValue`, `NegotiatingDealsValue`, `WonDealsValue` (sum Price).

### 3.6. Staff Performance

**Top Performing:**
1. Lấy tất cả staff có `Role = "Staff"` (chữ S viết hoa — lưu ý chính tả).
2. Với mỗi staff:
   - Lấy deals, contacts, leads của staff.
   - Tính `WonDeals`, `LostDeals`, `TotalDeals`.
   - `TotalRevenue = sum(Price)` deal WON.
   - `WinRate = WonDeals / TotalDeals * 100`.
   - `AverageDealValue = TotalRevenue / WonDeals.Count`.
3. Order by `WonDeals DESC`, take `limit`.
4. Trả về `StaffPerformanceListResponse`.

**Single Staff:**
- Tương tự nhưng filter theo `fromDate`, `toDate`.
- Lưu ý: `TasksCompleted` đang hardcode = 0.

### 3.7. Lead Conversion
- Lấy leads trong khoảng [fromDate, toDate].
- Đếm `ConvertedLeads` (Status = CONVERTED), `LostLeads` (Status = LOST).
- `ConversionRate = ConvertedLeads / Total * 100`.
- `AverageConversionDays = 0` (chưa implement).
- `ConversionBySources` = empty list (chưa implement).

### 3.8. Export Excel (EPPlus)
- Mỗi loại report (Deal, Lead, Customer) có 1 method riêng:
  - Tạo `ExcelPackage`, thêm worksheet.
  - Header: các cột tương ứng.
  - Body: từng row dữ liệu.
  - Style header (bold, nền xám, AutoFit).
  - Trả về `ExportReportResponse { FileName, DownloadUrl, ContentType, FileData (byte[]) }`.
- **Lưu ý:** `FileData` chứa binary nên GraphQL response có thể nặng — cân nhắc upload lên S3/blob storage rồi trả URL.

---

## 4. Business Rules

| Quy tắc | Mô tả |
|---------|-------|
| Role "Staff" trong GetTopPerforming | So sánh case-sensitive `"Staff"` — cần đảm bảo staff role trong DB đồng nhất. |
| Hardcode TasksCompleted | Tính năng chưa hoàn thiện. |
| Hardcode ConversionBySources | Chưa aggregate theo `Resource` của Lead. |
| Timezone | Mọi DateTime dùng UTC. Client convert theo timezone. |
| TotalRevenue | Tính trên TOÀN BỘ deal WON, không filter date — cần đọc kỹ để tránh hiểu nhầm. |

---

## 5. Tích hợp ngang

- **Deal/Lead/Customer/Contact**: tất cả aggregate đều query trực tiếp repository.
- **Staff**: lấy theo role để ranking.
- **EPPlus**: thư viện xuất Excel (NonCommercial license đã set trong `Program.cs`).
