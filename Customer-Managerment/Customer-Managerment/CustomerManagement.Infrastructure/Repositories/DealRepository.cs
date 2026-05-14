using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class DealRepository : IDealRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public DealRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
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
            return await context.Deals
                .Include(d => d.IdCustomerNavigation)
                .Include(d => d.IdStaffNavigation)
                .AsNoTracking()
                .ToListAsync();
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
            var deal = await context.Deals
                .FirstOrDefaultAsync(d => d.IdDeal == idDeal);

            if (deal == null)
                return false;

            deal.IsDeleted = true;
            deal.DeletedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }
    }
}