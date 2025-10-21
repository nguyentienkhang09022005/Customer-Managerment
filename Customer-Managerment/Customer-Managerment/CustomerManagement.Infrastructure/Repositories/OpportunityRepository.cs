using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class OpportunityRepository : IOpportunityRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public OpportunityRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<List<Opportunity>> GetListOpportunitiesAsync(Guid idUser)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Opportunities
                .AsNoTracking()
                .Where(o => o.IdUser == idUser)
                .IgnoreAutoIncludes()
                .ToListAsync();
        }

        public async Task<Opportunity> AddOpportunityAsync(OpportunityDomain opportunityDomain)
        {
            await using var context = _contextFactory.CreateDbContext();
            var opp = _mapper.Map<Opportunity>(opportunityDomain);

            opp.IdOpportunity = Guid.NewGuid();
            opp.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            await context.Opportunities.AddAsync(opp);
            await context.SaveChangesAsync();
            return opp;
        }

        public async Task<Opportunity> GetOpportunityByIdAsync(Guid idOpportunity)
        {
            await using var context = _contextFactory.CreateDbContext();
            var opp = await context.Opportunities
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.IdOpportunity == idOpportunity);

            if (opp == null)
                throw new NotFoundException("Không tìm thấy cơ hội!");
            return opp;
        }

        public async Task<Opportunity> UpdateOpportunityAsync(OpportunityDomain opportunityDomain, Opportunity opportunity)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.Attach(opportunity);

            _mapper.Map(opportunityDomain, opportunity);

            context.Entry(opportunity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return opportunity;
        }

        public async Task DeleteOpportunityAsync(Guid idOpportunity)
        {
            await using var context = _contextFactory.CreateDbContext();
            var opp = await context.Opportunities.FindAsync(idOpportunity);
            if (opp == null)
                throw new NotFoundException("Không tìm thấy cơ hội!");

            context.Opportunities.Remove(opp);
            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckOpportunityExistsAsync(Guid idOpportunity)
        {
            await using var context = _contextFactory.CreateDbContext();

            return await context.Opportunities
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .AnyAsync(o => o.IdOpportunity == idOpportunity);
        }

        public async Task<Opportunity> GetExistingOpportunityAsync(Guid idOpportunity)
        {
            await using var context = _contextFactory.CreateDbContext();

            var opportunity = await context.Opportunities
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .FirstOrDefaultAsync(o => o.IdOpportunity == idOpportunity);

            return opportunity;
        }
    }
}
