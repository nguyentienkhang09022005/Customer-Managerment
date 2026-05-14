using OfficeOpenXml;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class ExportHandler
    {
        private readonly ILeadRepository _leadRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IDealRepository _dealRepository;
        private readonly IStaffRepository _staffRepository;

        public ExportHandler(
            ILeadRepository leadRepository,
            ICustomerRepository customerRepository,
            IContactRepository contactRepository,
            IDealRepository dealRepository,
            IStaffRepository staffRepository)
        {
            _leadRepository = leadRepository;
            _customerRepository = customerRepository;
            _contactRepository = contactRepository;
            _dealRepository = dealRepository;
            _staffRepository = staffRepository;
        }

        public async Task<ExportReportResponse> ExportDealsToExcelAsync(DateTime fromDate, DateTime toDate)
        {
            var deals = await _dealRepository.GetListDealAsync();
            deals = deals.Where(d => d.CreatedAt >= fromDate && d.CreatedAt <= toDate).ToList();

            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Deals Report");

            worksheet.Cells[1, 1].Value = "Title";
            worksheet.Cells[1, 2].Value = "Price";
            worksheet.Cells[1, 3].Value = "Status";
            worksheet.Cells[1, 4].Value = "Created At";
            worksheet.Cells[1, 5].Value = "Staff";

            int row = 2;
            foreach (var deal in deals)
            {
                var staff = await _staffRepository.GetStaffByIdAsync(deal.IdStaff);
                worksheet.Cells[row, 1].Value = deal.Title;
                worksheet.Cells[row, 2].Value = deal.Price;
                worksheet.Cells[row, 3].Value = deal.Status ?? "UNKNOWN";
                worksheet.Cells[row, 4].Value = deal.CreatedAt.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 5].Value = staff?.Fullname ?? "Unknown";
                row++;
            }

            worksheet.Cells[1, 1, 1, 5].Style.Font.Bold = true;
            worksheet.Cells[1, 1, 1, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            worksheet.Cells[1, 1, 1, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            worksheet.Cells.AutoFitColumns();

            var fileName = $"DealsReport_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
            var fileData = package.GetAsByteArray();

            return new ExportReportResponse
            {
                FileName = fileName,
                DownloadUrl = $"/exports/{fileName}",
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileData = fileData
            };
        }

        public async Task<ExportReportResponse> ExportLeadsToExcelAsync(DateTime fromDate, DateTime toDate)
        {
            var leads = await _leadRepository.GetListLeadAsync();
            leads = leads.Where(l => l.CreatedAt >= fromDate && l.CreatedAt <= toDate).ToList();

            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Leads Report");

            worksheet.Cells[1, 1].Value = "Fullname";
            worksheet.Cells[1, 2].Value = "Email";
            worksheet.Cells[1, 3].Value = "Phone";
            worksheet.Cells[1, 4].Value = "Status";
            worksheet.Cells[1, 5].Value = "Created At";

            int row = 2;
            foreach (var lead in leads)
            {
                worksheet.Cells[row, 1].Value = lead.Fullname;
                worksheet.Cells[row, 2].Value = lead.Email;
                worksheet.Cells[row, 3].Value = lead.Phone;
                worksheet.Cells[row, 4].Value = LeadStatusConstant.ToString(lead.Status);
                worksheet.Cells[row, 5].Value = lead.CreatedAt.ToString("yyyy-MM-dd");
                row++;
            }

            worksheet.Cells[1, 1, 1, 5].Style.Font.Bold = true;
            worksheet.Cells[1, 1, 1, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            worksheet.Cells[1, 1, 1, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            worksheet.Cells.AutoFitColumns();

            var fileName = $"LeadsReport_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
            var fileData = package.GetAsByteArray();

            return new ExportReportResponse
            {
                FileName = fileName,
                DownloadUrl = $"/exports/{fileName}",
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileData = fileData
            };
        }

        public async Task<ExportReportResponse> ExportCustomersToExcelAsync(DateTime fromDate, DateTime toDate)
        {
            var customers = await _customerRepository.GetListCustomerAsync();
            customers = customers.Where(c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate).ToList();

            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Customers Report");

            worksheet.Cells[1, 1].Value = "Fullname";
            worksheet.Cells[1, 2].Value = "Email";
            worksheet.Cells[1, 3].Value = "Phone";
            worksheet.Cells[1, 4].Value = "Created At";

            int row = 2;
            foreach (var customer in customers)
            {
                worksheet.Cells[row, 1].Value = customer.Fullname;
                worksheet.Cells[row, 2].Value = customer.Email;
                worksheet.Cells[row, 3].Value = customer.Phone;
                worksheet.Cells[row, 4].Value = customer.CreatedAt.ToString("yyyy-MM-dd");
                row++;
            }

            worksheet.Cells[1, 1, 1, 4].Style.Font.Bold = true;
            worksheet.Cells[1, 1, 1, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            worksheet.Cells[1, 1, 1, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            worksheet.Cells.AutoFitColumns();

            var fileName = $"CustomersReport_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
            var fileData = package.GetAsByteArray();

            return new ExportReportResponse
            {
                FileName = fileName,
                DownloadUrl = $"/exports/{fileName}",
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileData = fileData
            };
        }
    }
}