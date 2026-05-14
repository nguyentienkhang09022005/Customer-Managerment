using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
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
            return await context.Persons
                .Where(p => p.Discriminator == PersonType.Lead)
                .AsNoTracking()
                .ToListAsync();
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
            var lead = await context.Persons
                .FirstOrDefaultAsync(p => p.Id == idLead && p.Discriminator == PersonType.Lead);

            if (lead == null)
                return false;

            lead.IsDeleted = true;
            lead.DeletedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreLeadAsync(Guid idLead)
        {
            await using var context = _contextFactory.CreateDbContext();
            var lead = await context.Persons
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == idLead && p.Discriminator == PersonType.Lead);

            if (lead == null || !lead.IsDeleted)
                return false;

            lead.IsDeleted = false;
            lead.DeletedAt = null;

            await context.SaveChangesAsync();
            return true;
        }
    }
}