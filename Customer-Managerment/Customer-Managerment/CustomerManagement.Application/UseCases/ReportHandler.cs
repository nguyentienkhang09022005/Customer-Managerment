using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Microsoft.EntityFrameworkCore;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class ReportHandler
    {
        private readonly ILeadRepository _leadRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IDealRepository _dealRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IMapper _mapper;

        public ReportHandler(
            ILeadRepository leadRepository,
            ICustomerRepository customerRepository,
            IContactRepository contactRepository,
            IDealRepository dealRepository,
            ITaskRepository taskRepository,
            IStaffRepository staffRepository,
            IMapper mapper)
        {
            _leadRepository = leadRepository;
            _customerRepository = customerRepository;
            _contactRepository = contactRepository;
            _dealRepository = dealRepository;
            _taskRepository = taskRepository;
            _staffRepository = staffRepository;
            _mapper = mapper;
        }

        public async Task<DashboardResponse> GetDashboardSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            var leads = await _leadRepository.GetListLeadAsync();
            var customers = await _customerRepository.GetListCustomerAsync();
            var deals = await _dealRepository.GetListDealAsync();
            var contacts = await _contactRepository.GetListContactAsync();

            var leadsThisMonth = leads.Count(l => l.CreatedAt >= fromDate && l.CreatedAt <= toDate);
            var customersThisMonth = customers.Count(c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate);
            var dealsWonThisMonth = deals.Count(d => d.Status == StatuDealConstant.DealWon && d.CreatedAt >= fromDate && d.CreatedAt <= toDate);
            var dealsLostThisMonth = deals.Count(d => d.Status == StatuDealConstant.DealLost && d.CreatedAt >= fromDate && d.CreatedAt <= toDate);

            var wonDeals = deals.Where(d => d.Status == StatuDealConstant.DealWon).ToList();
            var totalRevenue = wonDeals.Sum(d => d.Price);
            var avgDealValue = wonDeals.Count > 0 ? totalRevenue / wonDeals.Count : 0;
            var conversionRate = leads.Count > 0 ? (decimal)customers.Count / leads.Count * 100 : 0;

            var activeDeals = deals.Count(d => d.Status == StatuDealConstant.DealOpen || d.Status == StatuDealConstant.DealNegotiating);

            return new DashboardResponse
            {
                TotalLeads = leads.Count,
                TotalCustomers = customers.Count,
                ActiveDeals = activeDeals,
                TotalContacts = contacts.Count,
                TotalRevenue = totalRevenue,
                ConversionRate = Math.Round(conversionRate, 2),
                AverageDealValue = avgDealValue,
                LeadsCreatedThisMonth = leadsThisMonth,
                CustomersCreatedThisMonth = customersThisMonth,
                DealsWonThisMonth = dealsWonThisMonth,
                DealsLostThisMonth = dealsLostThisMonth
            };
        }

        public async Task<RevenueChartResponse> GetRevenueChartAsync(DateTime fromDate, DateTime toDate, string groupBy)
        {
            var deals = await _dealRepository.GetListDealAsync();
            deals = deals.Where(d => d.CreatedAt >= fromDate && d.CreatedAt <= toDate).ToList();

            var wonDeals = deals.Where(d => d.Status == StatuDealConstant.DealWon).ToList();
            var lostDeals = deals.Where(d => d.Status == StatuDealConstant.DealLost).ToList();
            var openDeals = deals.Where(d => d.Status == StatuDealConstant.DealOpen || d.Status == StatuDealConstant.DealNegotiating).ToList();

            var dataPoints = new List<RevenueDataPoint>();

            if (groupBy.ToLower() == "day")
            {
                var days = Enumerable.Range(0, (toDate - fromDate).Days + 1)
                    .Select(d => fromDate.AddDays(d))
                    .ToList();

                foreach (var day in days)
                {
                    var dayWon = wonDeals.Where(d => d.CreatedAt.Date == day.Date).Sum(d => d.Price);
                    var dayLost = lostDeals.Where(d => d.CreatedAt.Date == day.Date).Sum(d => d.Price);
                    var dayPipeline = openDeals.Where(d => d.CreatedAt.Date == day.Date).Sum(d => d.Price);

                    dataPoints.Add(new RevenueDataPoint
                    {
                        Date = day,
                        WonAmount = dayWon,
                        LostAmount = dayLost,
                        PipelineValue = dayPipeline
                    });
                }
            }
            else
            {
                var months = Enumerable.Range(0, 13)
                    .Select(m => new DateTime(fromDate.Year, fromDate.Month, 1).AddMonths(m))
                    .Where(d => d <= toDate)
                    .ToList();

                foreach (var month in months)
                {
                    var monthStart = month;
                    var monthEnd = month.AddMonths(1).AddDays(-1);

                    var monthWon = wonDeals.Where(d => d.CreatedAt >= monthStart && d.CreatedAt <= monthEnd).Sum(d => d.Price);
                    var monthLost = lostDeals.Where(d => d.CreatedAt >= monthStart && d.CreatedAt <= monthEnd).Sum(d => d.Price);
                    var monthPipeline = openDeals.Where(d => d.CreatedAt >= monthStart && d.CreatedAt <= monthEnd).Sum(d => d.Price);

                    dataPoints.Add(new RevenueDataPoint
                    {
                        Date = month,
                        WonAmount = monthWon,
                        LostAmount = monthLost,
                        PipelineValue = monthPipeline
                    });
                }
            }

            return new RevenueChartResponse
            {
                DataPoints = dataPoints,
                TotalRevenue = wonDeals.Sum(d => d.Price),
                TotalLost = lostDeals.Sum(d => d.Price)
            };
        }

        public async Task<PipelineFunnelResponse> GetPipelineFunnelAsync()
        {
            var deals = await _dealRepository.GetListDealAsync();

            var openDeals = deals.Where(d => d.Status == StatuDealConstant.DealOpen).ToList();
            var negotiatingDeals = deals.Where(d => d.Status == StatuDealConstant.DealNegotiating).ToList();
            var wonDeals = deals.Where(d => d.Status == StatuDealConstant.DealWon).ToList();

            return new PipelineFunnelResponse
            {
                OpenDealsCount = openDeals.Count,
                NegotiatingDealsCount = negotiatingDeals.Count,
                WonDealsCount = wonDeals.Count,
                OpenDealsValue = openDeals.Sum(d => d.Price),
                NegotiatingDealsValue = negotiatingDeals.Sum(d => d.Price),
                WonDealsValue = wonDeals.Sum(d => d.Price)
            };
        }

        public async Task<StaffPerformanceListResponse> GetTopPerformingStaffAsync(int limit)
        {
            var staffList = await _staffRepository.GetStaffByRoleAsync("Staff");
            var staff = staffList.ToList();

            var staffPerformances = new List<StaffPerformanceResponse>();

            foreach (var s in staff)
            {
                var deals = await _dealRepository.GetListDealAsync();
                deals = deals.Where(d => d.IdStaff == s.Id).ToList();
                var contacts = await _contactRepository.GetListContactAsync();
                contacts = contacts.Where(c => c.IdStaff == s.Id).ToList();
                var leads = await _leadRepository.GetListLeadAsync();
                leads = leads.Where(l => l.Id == s.Id).ToList();

                var wonDeals = deals.Where(d => d.Status == StatuDealConstant.DealWon).ToList();
                var lostDeals = deals.Where(d => d.Status == StatuDealConstant.DealLost).ToList();
                var totalDeals = deals.Count;
                var totalRevenue = wonDeals.Sum(d => d.Price);
                var winRate = totalDeals > 0 ? (decimal)wonDeals.Count / totalDeals * 100 : 0;
                var avgDealValue = wonDeals.Count > 0 ? totalRevenue / wonDeals.Count : 0;

                staffPerformances.Add(new StaffPerformanceResponse
                {
                    IdStaff = s.Id,
                    StaffName = s.Fullname,
                    TotalDealsCreated = totalDeals,
                    WonDeals = wonDeals.Count,
                    LostDeals = lostDeals.Count,
                    WinRate = Math.Round(winRate, 2),
                    TotalRevenue = totalRevenue,
                    AverageDealValue = avgDealValue,
                    ContactsCreated = contacts.Count,
                    LeadsCreated = leads.Count,
                    TasksCompleted = 0
                });
            }

            var topStaff = staffPerformances
                .OrderByDescending(sp => sp.WonDeals)
                .Take(limit)
                .ToList();

            return new StaffPerformanceListResponse
            {
                StaffPerformances = topStaff,
                TotalStaff = staff.Count
            };
        }

        public async Task<StaffPerformanceResponse> GetStaffPerformanceAsync(Guid idStaff, DateTime fromDate, DateTime toDate)
        {
            var staff = await _staffRepository.GetStaffByIdAsync(idStaff);
            if (staff == null)
                return new StaffPerformanceResponse { IdStaff = idStaff, StaffName = "Unknown" };

            var deals = await _dealRepository.GetListDealAsync();
            deals = deals.Where(d => d.IdStaff == idStaff && d.CreatedAt >= fromDate && d.CreatedAt <= toDate).ToList();
            var contacts = await _contactRepository.GetListContactAsync();
            contacts = contacts.Where(c => c.IdStaff == idStaff && c.CreatedAt >= fromDate && c.CreatedAt <= toDate).ToList();
            var leads = await _leadRepository.GetListLeadAsync();
            leads = leads.Where(l => l.Id == idStaff && l.CreatedAt >= fromDate && l.CreatedAt <= toDate).ToList();

            var wonDeals = deals.Where(d => d.Status == StatuDealConstant.DealWon).ToList();
            var lostDeals = deals.Where(d => d.Status == StatuDealConstant.DealLost).ToList();
            var totalDeals = deals.Count;
            var totalRevenue = wonDeals.Sum(d => d.Price);
            var winRate = totalDeals > 0 ? (decimal)wonDeals.Count / totalDeals * 100 : 0;
            var avgDealValue = wonDeals.Count > 0 ? totalRevenue / wonDeals.Count : 0;

            return new StaffPerformanceResponse
            {
                IdStaff = idStaff,
                StaffName = staff.Fullname,
                TotalDealsCreated = totalDeals,
                WonDeals = wonDeals.Count,
                LostDeals = lostDeals.Count,
                WinRate = Math.Round(winRate, 2),
                TotalRevenue = totalRevenue,
                AverageDealValue = avgDealValue,
                ContactsCreated = contacts.Count,
                LeadsCreated = leads.Count,
                TasksCompleted = 0
            };
        }

        public async Task<LeadConversionResponse> GetLeadConversionReportAsync(DateTime fromDate, DateTime toDate)
        {
            var leads = await _leadRepository.GetListLeadAsync();
            leads = leads.Where(l => l.CreatedAt >= fromDate && l.CreatedAt <= toDate).ToList();

            var convertedLeads = leads.Count(l => l.Status == LeadStatusConstant.LeadStatusConverted);
            var lostLeads = leads.Count(l => l.Status == LeadStatusConstant.LeadStatusLost);
            var conversionRate = leads.Count > 0 ? (decimal)convertedLeads / leads.Count * 100 : 0;

            return new LeadConversionResponse
            {
                TotalLeads = leads.Count,
                ConvertedLeads = convertedLeads,
                LostLeads = lostLeads,
                ConversionRate = Math.Round(conversionRate, 2),
                AverageConversionDays = 0,
                ConversionBySources = new List<ConversionBySource>()
            };
        }
    }
}