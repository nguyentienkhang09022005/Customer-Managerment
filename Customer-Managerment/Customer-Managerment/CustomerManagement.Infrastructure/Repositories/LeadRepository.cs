using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class LeadRepository : ILeadRepository
    {
        private const int MaxUnboundedRecords = 1000;

        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<LeadRepository> _logger;

        public LeadRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory,
                              IMapper mapper,
                              ILogger<LeadRepository> logger)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Person> AddLeadAsync(Person lead)
        {
            await using var context = _contextFactory.CreateDbContext();

            lead.Id = Guid.NewGuid();
            lead.Discriminator = PersonType.Lead;
            lead.CreatedAt = DateTime.UtcNow;
            lead.IsDeleted = false;

            await context.Persons.AddAsync(lead);
            await context.SaveChangesAsync();
            return lead;
        }

        public async Task<Person?> GetLeadByIdAsync(Guid idLead)
        {
            await using var context = _contextFactory.CreateDbContext();
            var lead = await context.Persons
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == idLead && p.Discriminator == PersonType.Lead);

            if (lead == null)
                throw new NotFoundException("Không tìm thấy Lead!");

            return lead;
        }

        public IQueryable<Person> GetListLead()
        {
            var context = _contextFactory.CreateDbContext();
            return context.Persons
                .Where(p => p.Discriminator == PersonType.Lead)
                .AsNoTracking();
        }

        public async Task<List<Person>> GetListLeadAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            var total = await context.Persons.CountAsync(p => p.Discriminator == PersonType.Lead);
            if (total > MaxUnboundedRecords)
            {
                _logger.LogWarning("GetListLeadAsync returned {Returned}/{Total} records (hard cap {Cap}). Use GetListLeadPagedAsync for full data.",
                    MaxUnboundedRecords, total, MaxUnboundedRecords);
            }
            return await context.Persons
                .Where(p => p.Discriminator == PersonType.Lead)
                .AsNoTracking()
                .Take(MaxUnboundedRecords)
                .ToListAsync();
        }

        public async Task<(List<Person> Items, int TotalCount)> GetListLeadPagedAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 200) pageSize = 200;

            await using var context = _contextFactory.CreateDbContext();
            var query = context.Persons.Where(p => p.Discriminator == PersonType.Lead);
            var total = await query.CountAsync();
            var items = await query
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, total);
        }

        public IQueryable<Person> GetLeadById(Guid idLead)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Persons
                .Where(p => p.Id == idLead && p.Discriminator == PersonType.Lead)
                .AsNoTracking();
        }

        public async Task<Person?> UpdateLeadAsync(Person lead)
        {
            await using var context = _contextFactory.CreateDbContext();
            var existingLead = await context.Persons
                .FirstOrDefaultAsync(p => p.Id == lead.Id && p.Discriminator == PersonType.Lead);

            if (existingLead == null)
                return null;

            existingLead.Fullname = lead.Fullname;
            existingLead.Email = lead.Email;
            existingLead.Phone = lead.Phone;
            existingLead.Location = lead.Location;
            existingLead.Resource = lead.Resource;
            existingLead.Discriminator = lead.Discriminator;
            existingLead.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return existingLead;
        }

        public async Task<bool> CheckLeadExistsAsync(Guid idLead)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Persons
                .AnyAsync(p => p.Id == idLead && p.Discriminator == PersonType.Lead);
        }

        public async Task<bool> CheckPersonByEmailAsync(string email)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Persons
                .IgnoreQueryFilters()
                .AnyAsync(p => p.Email == email);
        }

        public async Task<int> GetTotalLeadsAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Persons
                .CountAsync(p => p.Discriminator == PersonType.Lead);
        }

        public async Task<bool> SoftDeleteLeadAsync(Guid idLead)
        {
            await using var context = _contextFactory.CreateDbContext();
            var rows = await context.Persons
                .Where(p => p.Id == idLead && p.Discriminator == PersonType.Lead)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.IsDeleted, true)
                    .SetProperty(p => p.DeletedAt, DateTime.UtcNow));
            return rows > 0;
        }

        public async Task<bool> RestoreLeadAsync(Guid idLead)
        {
            await using var context = _contextFactory.CreateDbContext();
            var rows = await context.Persons
                .IgnoreQueryFilters()
                .Where(p => p.Id == idLead && p.Discriminator == PersonType.Lead && p.IsDeleted)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.IsDeleted, false)
                    .SetProperty(p => p.DeletedAt, (DateTime?)null));
            return rows > 0;
        }
    }
}