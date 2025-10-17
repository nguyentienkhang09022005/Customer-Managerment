using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;
using SendGrid.Helpers.Mail;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public CompanyRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, 
                                 IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<List<Company>> GetListCompanyAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Companies
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .ToListAsync();
        }

        public async Task<Company> GetCompanyByIdAsync(Guid idCompany)
        {
            await using var context = _contextFactory.CreateDbContext();
            var company = await context.Companies
                    .AsNoTracking()
                    .IgnoreAutoIncludes()
                    .FirstOrDefaultAsync(u => u.IdCompany == idCompany);

            if (company == null)
                throw new NotFoundException("Không tìm thấy thông tin công ty!");
            return company;
        }

        public async Task<Company> AddCompanyAsync(CompanyDomain companyDomain)
        {
            var company = _mapper.Map<Company>(companyDomain);
            company.IdCompany = Guid.NewGuid();
            await using var context = _contextFactory.CreateDbContext();

            context.Companies.Add(company);
            await context.SaveChangesAsync();
            return company;
        }

        public async Task<Company> UpdateCompanyAsync(CompanyDomain companyDomain, Guid idCompany)
        {
            await using var context = _contextFactory.CreateDbContext();
            var company = await context.Companies.FindAsync(idCompany);
            if (company == null)
                throw new NotFoundException("Không tìm thấy công ty!");

            // Cập nhật các thuộc tính của company
            _mapper.Map(companyDomain, company);
            await context.SaveChangesAsync();
            return company;
        }

        public async Task DeleteCompanyAsync(Guid idCompany)
        {
            await using var context = _contextFactory.CreateDbContext();
            var company = await context.Companies.FindAsync(idCompany);
            if (company == null)
                throw new NotFoundException("Không tìm thấy công ty!");

            context.Companies.Remove(company);
            await context.SaveChangesAsync();
        }
    }
}
