using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
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

        public async Task<DealDomain> AddDealAsync(DealDomain dealDomain)
        {
            await using var context = _contextFactory.CreateDbContext();
            var deal = _mapper.Map<Deal>(dealDomain);

            deal.IdDeal = Guid.NewGuid();
            deal.Status = StatuDealConstant.DealPending;
            deal.CreatedAt = DateTime.Now;

            await context.Deals.AddAsync(deal);
            await context.SaveChangesAsync();
            return _mapper.Map<DealDomain>(deal);
        }

        public async Task<bool> CheckDealExistsAsync(Guid idDeal)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Deals
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .AnyAsync(d => d.IdDeal == idDeal);
        }

        public async Task DeleteDealAsync(Guid idDeal)
        {
            await using var context = _contextFactory.CreateDbContext();
            var deal = await context.Deals.FindAsync(idDeal);
            if (deal == null)
                throw new NotFoundException("Không tìm thấy deal!");

            context.Deals.Remove(deal);
            await context.SaveChangesAsync();
        }

        public IQueryable<DealDomain> GetDealById(Guid idDeal)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Deals
                .Where(d => d.IdDeal == idDeal)
                .Include(d => d.IdCustomerNavigation)
                .Include(d => d.IdStaffNavigation)
                .ProjectTo<DealDomain>(_mapper.ConfigurationProvider)
                .AsNoTracking();
        }

        public async Task<DealDomain?> GetDealByIdAsync(Guid idDeal)
        {
            await using var context = _contextFactory.CreateDbContext();
            var deal = await context.Deals
                    .AsNoTracking()
                    .IgnoreAutoIncludes()
                    .FirstOrDefaultAsync(d => d.IdDeal == idDeal);

            if (deal == null)
                throw new NotFoundException("Không tìm thấy deal!");
            return _mapper.Map<DealDomain>(deal);
        }

        public IQueryable<DealDomain> GetListDeal()
        {
            var context = _contextFactory.CreateDbContext();
            return context.Deals
                .Include(d => d.IdCustomerNavigation)
                .Include(d => d.IdStaffNavigation)
                .ProjectTo<DealDomain>(_mapper.ConfigurationProvider)
                .AsNoTracking();
        }

        public async Task<List<DealDomain>> GetListDealAsync()
        {
            await using var context = _contextFactory.CreateDbContext();

            return await context.Deals
                .Include(d => d.IdCustomerNavigation)
                .Include(d => d.IdStaffNavigation)
                .ProjectTo<DealDomain>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<DealDomain?> UpdateDealAsync(DealDomain dealDomain)
        {
            await using var context = _contextFactory.CreateDbContext();
            var deal= await context.Deals.FindAsync(dealDomain.IdDeal);
            if (deal == null) return null;

            // Cập nhật các thuộc tính của deal
            _mapper.Map(dealDomain, deal);
            context.Entry(deal).Property(c => c.Title).IsModified = false;
            context.Entry(deal).Property(c => c.Content).IsModified = false;
            context.Entry(deal).Property(c => c.Price).IsModified = false;

            await context.SaveChangesAsync();
            return _mapper.Map<DealDomain>(deal);
        }

        public async Task<decimal> GetTotalProfitAsync()
        {
            await using var context = _contextFactory.CreateDbContext();

            return await context.Deals
                .AsNoTracking()
                .Where(d => d.Status == StatuDealConstant.DealWon)
                .SumAsync(d => (decimal?)d.Price) ?? 0m;
        }


        public async Task<int> getTotalDealsAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Deals.CountAsync();
        }

        public async Task<QuantityStatisticsDetailDealResponse> QuantityStatisticsDetailDealResponse()
        {
            await using var context = _contextFactory.CreateDbContext();
            var totalPending = await context.Deals
                .AsNoTracking()
                .CountAsync(c => c.Status == StatuDealConstant.DealPending);
            var totalWon = await context.Deals
                .AsNoTracking()
                .CountAsync(c => c.Status == StatuDealConstant.DealWon);
            var totalLost = await context.Deals
                .AsNoTracking()
                .CountAsync(c => c.Status == StatuDealConstant.DealLost);
            return new QuantityStatisticsDetailDealResponse
            {
                QuantityDealsPending = totalPending,
                QuantityDealsWon = totalWon,
                QuantityDealsLost = totalLost
            };
        }
    }
}
