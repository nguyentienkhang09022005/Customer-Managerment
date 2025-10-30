using AutoMapper;
using AutoMapper.QueryableExtensions;
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

        public async Task<LeadDomain> AddLeadAsync(LeadDomain leadDomain)
        {
            await using var context = _contextFactory.CreateDbContext();

            if (leadDomain.personDomain == null)
                throw new BadRequestException("Thông tin Person không hợp lệ!");

            var sharedId = Guid.NewGuid();

            var personEntity = _mapper.Map<Person>(leadDomain.personDomain);
            personEntity.IdPerson = sharedId;
            await context.Person.AddAsync(personEntity);
            await context.SaveChangesAsync();

            var leadEntity = new Lead
            {
                IdLead = sharedId,
                Resource = leadDomain.Resource,
                CreatedAt = DateTime.Now,
                IdLeadNavigation = personEntity
            };

            await context.Leads.AddAsync(leadEntity);
            await context.SaveChangesAsync();

            return _mapper.Map<LeadDomain>(leadEntity);
        }

        public async Task<LeadDomain?> GetLeadByIdAsync(Guid idLead)
        {
            await using var context = _contextFactory.CreateDbContext();
            var lead = await context.Leads
                .Include(l => l.IdLeadNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.IdLead == idLead);

            if (lead == null) throw new NotFoundException("Không tìm thấy Lead!");
            return _mapper.Map<LeadDomain>(lead);
        }

        public IQueryable<LeadDomain> GetListLead()
        {
            var context = _contextFactory.CreateDbContext();
            return context.Leads
                .Include(l => l.IdLeadNavigation)
                .ProjectTo<LeadDomain>(_mapper.ConfigurationProvider)
                .AsNoTracking();
        }

        public IQueryable<LeadDomain> GetLeadById(Guid idLead)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Leads
                .Where(l => l.IdLead == idLead)
                .Include(l => l.IdLeadNavigation)
                .ProjectTo<LeadDomain>(_mapper.ConfigurationProvider)
                .AsNoTracking();
        }

        public async Task<LeadDomain?> UpdateLeadAsync(LeadDomain leadDomain)
        {
            await using var context = _contextFactory.CreateDbContext();
            var existingLead = await context.Leads
                .Include(l => l.IdLeadNavigation)
                .FirstOrDefaultAsync(l => l.IdLead == leadDomain.IdLead);

            if (existingLead == null) return null;

            _mapper.Map(leadDomain, existingLead);

            if (leadDomain.personDomain != null && existingLead.IdLeadNavigation != null)
            {
                _mapper.Map(leadDomain.personDomain, existingLead.IdLeadNavigation);
            }

            await context.SaveChangesAsync();
            return _mapper.Map<LeadDomain>(existingLead);
        }

        public async Task DeleteLeadAsync(Guid idLead)
        {
            await using var context = _contextFactory.CreateDbContext();
            var lead = await context.Leads
                .Include(l => l.IdLeadNavigation)
                .FirstOrDefaultAsync(l => l.IdLead == idLead);

            if (lead == null) throw new NotFoundException("Không tìm thấy Lead!");

            context.Person.Remove(lead.IdLeadNavigation);
            context.Leads.Remove(lead);
            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckLeadExistsAsync(Guid idLead)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Leads.AnyAsync(l => l.IdLead == idLead);
        }

        public async Task<bool> checkPersonByEmailAsync(string email)
        {
            await using var context = _contextFactory.CreateDbContext();
            var checkPerson = await context.Person
                .IgnoreAutoIncludes()
                .AsNoTracking()
                .AnyAsync(l => l.Email == email);
            return checkPerson;
        }
    }
}
