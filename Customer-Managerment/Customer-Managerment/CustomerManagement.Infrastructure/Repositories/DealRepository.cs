using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class DealRepository : IDealRepository
    {
        // Hard cap prevents OOM in production if a caller forgets to paginate.
        private const int MaxUnboundedRecords = 1000;

        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<DealRepository> _logger;

        public DealRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory,
                              IMapper mapper,
                              ILogger<DealRepository> logger)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Deal> AddDealAsync(Deal deal)
        {
            await using var context = _contextFactory.CreateDbContext();

            deal.IdDeal = Guid.NewGuid();
            deal.Status = StatuDealConstant.DealOpen;
            deal.CreatedAt = DateTime.UtcNow;
            deal.IsDeleted = false;

            await context.Deals.AddAsync(deal);
            await context.SaveChangesAsync();
            return deal;
        }

        public async Task<Deal?> GetDealByIdAsync(Guid idDeal)
        {
            await using var context = _contextFactory.CreateDbContext();
            var deal = await context.Deals
                    .IgnoreQueryFilters()
                    .Include(d => d.IdCustomerNavigation)
                    .Include(d => d.IdStaffNavigation)
                    .FirstOrDefaultAsync(d => d.IdDeal == idDeal);

            if (deal == null)
                throw new NotFoundException("Không tìm thấy deal!");

            return deal;
        }

        public IQueryable<Deal> GetListDeal()
        {
            var context = _contextFactory.CreateDbContext();
            return context.Deals
                .Include(d => d.IdCustomerNavigation)
                .Include(d => d.IdStaffNavigation)
                .AsNoTracking();
        }

        public async Task<List<Deal>> GetListDealAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            var total = await context.Deals.CountAsync();
            if (total > MaxUnboundedRecords)
            {
                _logger.LogWarning("GetListDealAsync returned {Returned}/{Total} records (hard cap {Cap}). Use GetListDealPagedAsync for full data.",
                    MaxUnboundedRecords, total, MaxUnboundedRecords);
            }
            return await context.Deals
                .Include(d => d.IdCustomerNavigation)
                .Include(d => d.IdStaffNavigation)
                .AsNoTracking()
                .Take(MaxUnboundedRecords)
                .ToListAsync();
        }

        public async Task<(List<Deal> Items, int TotalCount)> GetListDealPagedAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 200) pageSize = 200;

            await using var context = _contextFactory.CreateDbContext();
            var total = await context.Deals.CountAsync();
            var items = await context.Deals
                .Include(d => d.IdCustomerNavigation)
                .Include(d => d.IdStaffNavigation)
                .AsNoTracking()
                .OrderByDescending(d => d.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, total);
        }

        public IQueryable<Deal> GetDealById(Guid idDeal)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Deals
                .Where(d => d.IdDeal == idDeal)
                .Include(d => d.IdCustomerNavigation)
                .Include(d => d.IdStaffNavigation)
                .AsNoTracking();
        }

        public async Task<Deal?> UpdateDealAsync(Deal deal)
        {
            await using var context = _contextFactory.CreateDbContext();
            var existingDeal = await context.Deals
                .Include(d => d.IdCustomerNavigation)
                .Include(d => d.IdStaffNavigation)
                .FirstOrDefaultAsync(d => d.IdDeal == deal.IdDeal);

            if (existingDeal == null)
                return null;

            existingDeal.Title = deal.Title;
            existingDeal.Content = deal.Content;
            existingDeal.Price = deal.Price;
            existingDeal.Status = deal.Status;
            existingDeal.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return existingDeal;
        }

        public async Task<bool> CheckDealExistsAsync(Guid idDeal)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Deals
                .AnyAsync(d => d.IdDeal == idDeal);
        }

        public async Task<decimal> GetTotalProfitAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Deals
                .Where(d => d.Status == StatuDealConstant.DealWon)
                .SumAsync(d => (decimal?)d.Price) ?? 0m;
        }

        public async Task<int> GetTotalDealsAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Deals.CountAsync();
        }

        public async Task<QuantityStatisticsDetailDealResponse> QuantityStatisticsDetailDealResponse()
        {
            await using var context = _contextFactory.CreateDbContext();
            var totalOpen = await context.Deals
                .CountAsync(c => c.Status == StatuDealConstant.DealOpen);
            var totalWon = await context.Deals
                .CountAsync(c => c.Status == StatuDealConstant.DealWon);
            var totalLost = await context.Deals
                .CountAsync(c => c.Status == StatuDealConstant.DealLost);

            return new QuantityStatisticsDetailDealResponse
            {
                QuantityDealsPending = totalOpen,
                QuantityDealsWon = totalWon,
                QuantityDealsLost = totalLost
            };
        }

        public async Task<bool> SoftDeleteDealAsync(Guid idDeal)
        {
            await using var context = _contextFactory.CreateDbContext();
            var rows = await context.Deals
                .Where(d => d.IdDeal == idDeal)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(d => d.IsDeleted, true)
                    .SetProperty(d => d.DeletedAt, DateTime.UtcNow));
            return rows > 0;
        }
    }
}