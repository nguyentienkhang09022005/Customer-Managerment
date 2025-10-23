using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class LeadRepository : ILeadRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public LeadRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<List<Lead>> GetAllLeadsAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Leads.AsNoTracking().IgnoreAutoIncludes().ToListAsync();
        }

        public async Task<List<Lead>> GetLeadsByCampaignAsync(Guid idCampaign)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Leads
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Where(l => l.IdCampaign == idCampaign)
                .ToListAsync();
        }

        public async Task<Lead> GetLeadByIdAsync(Guid idLead)
        {
            await using var context = _contextFactory.CreateDbContext();
            var lead = await context.Leads
                .IgnoreAutoIncludes()
                .FirstOrDefaultAsync(l => l.IdLead == idLead);

            if (lead == null)
                throw new NotFoundException("Không tìm thấy lead!");
            return lead;
        }

        public async Task<Lead> AddLeadAsync(LeadDomain leadDomain)
        {
            await using var context = _contextFactory.CreateDbContext();
            var lead = _mapper.Map<Lead>(leadDomain);
            lead.IdLead = Guid.NewGuid();
            lead.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            await context.Leads.AddAsync(lead);
            await context.SaveChangesAsync();
            return lead;
        }

        public async Task<Lead> UpdateLeadAsync(LeadDomain leadDomain, Lead leadEntity)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.Attach(leadEntity);
            _mapper.Map(leadDomain, leadEntity);
            context.Entry(leadEntity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return leadEntity;
        }

        public async Task DeleteLeadAsync(Guid idLead)
        {
            await using var context = _contextFactory.CreateDbContext();
            var lead = await context.Leads.FindAsync(idLead);
            if (lead == null)
                throw new NotFoundException("Không tìm thấy lead!");

            context.Leads.Remove(lead);
            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckLeadExistsAsync(Guid idLead)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Leads.AnyAsync(l => l.IdLead == idLead);
        }

        public async Task<Lead> GetExistingLeadAsync(Guid idLead)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Leads
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .FirstOrDefaultAsync(l => l.IdLead == idLead);
        }
    }
}
