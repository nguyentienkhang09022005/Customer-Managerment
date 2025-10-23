using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class CaseRepository : ICaseRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public CaseRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<List<Case>> GetListCasesAsync(Guid idUser)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Cases
                .AsNoTracking()
                .Where(c => c.IdUser == idUser)
                .Include(d => d.IdUserNavigation)
                .Include(a => a.IdCustomerNavigation)
                .ToListAsync();
        }

        public async Task<Case> AddCaseAsync(CaseDomain caseDomain)
        {
            await using var context = _contextFactory.CreateDbContext();
            var newCase = _mapper.Map<Case>(caseDomain);
            newCase.IdCase = Guid.NewGuid();
            newCase.Status = "Open";
            newCase.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            await context.Cases.AddAsync(newCase);
            await context.SaveChangesAsync();
            return newCase;
        }
        public async Task<Case> UpdateCaseAsync(CaseDomain caseDomain, Case caseEntity)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.Attach(caseEntity);
            _mapper.Map(caseDomain, caseEntity);
            caseEntity.ResolveAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            context.Entry(caseEntity).State = EntityState.Modified;

            await context.SaveChangesAsync();
            return caseEntity;
        }

        public async Task<Case> GetCaseByIdAsync(Guid idCase)
        {
            await using var context = _contextFactory.CreateDbContext();
            var result = await context.Cases
                .Include(d => d.IdUserNavigation)
                .Include(a => a.IdCustomerNavigation)
                .FirstOrDefaultAsync(c => c.IdCase == idCase);
            if (result == null)
                throw new NotFoundException("Không tìm thấy Case!");
            return result;
        }

        public async Task DeleteCaseAsync(Guid idCase)
        {
            await using var context = _contextFactory.CreateDbContext();
            var caseEntity = await context.Cases.FindAsync(idCase);
            if (caseEntity == null)
                throw new NotFoundException("Không tìm thấy Case!");

            context.Cases.Remove(caseEntity);
            await context.SaveChangesAsync();
        }

        public async Task<Case> GetExistingCaseAsync(Guid idCase)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Cases
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdCase == idCase);
        }
    }
}
