using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public CustomerRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<Person> AddCustomerAsync(Person customer)
        {
            await using var context = _contextFactory.CreateDbContext();

            customer.Id = Guid.NewGuid();
            customer.Discriminator = PersonType.Customer;
            customer.CreatedAt = DateTime.UtcNow;
            customer.IsDeleted = false;

            await context.Persons.AddAsync(customer);
            await context.SaveChangesAsync();
            return customer;
        }

        public async Task<Person?> GetCustomerByIdAsync(Guid idCustomer)
        {
            await using var context = _contextFactory.CreateDbContext();
            var customer = await context.Persons
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == idCustomer && p.Discriminator == PersonType.Customer);

            if (customer == null)
                throw new NotFoundException("Không tìm thấy Customer!");

            return customer;
        }

        public IQueryable<Person> GetListCustomer()
        {
            var context = _contextFactory.CreateDbContext();
            return context.Persons
                .Where(p => p.Discriminator == PersonType.Customer)
                .AsNoTracking();
        }

        public async Task<List<Person>> GetListCustomerAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Persons
                .Where(p => p.Discriminator == PersonType.Customer)
                .AsNoTracking()
                .ToListAsync();
        }

        public IQueryable<Person> GetCustomerById(Guid idCustomer)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Persons
                .Where(p => p.Id == idCustomer && p.Discriminator == PersonType.Customer)
                .AsNoTracking();
        }

        public async Task<Person?> UpdateCustomerAsync(Person customer)
        {
            await using var context = _contextFactory.CreateDbContext();
            var existingCustomer = await context.Persons
                .FirstOrDefaultAsync(p => p.Id == customer.Id && p.Discriminator == PersonType.Customer);

            if (existingCustomer == null)
                return null;

            existingCustomer.Fullname = customer.Fullname;
            existingCustomer.Email = customer.Email;
            existingCustomer.Phone = customer.Phone;
            existingCustomer.Location = customer.Location;
            existingCustomer.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return existingCustomer;
        }

        public async Task<bool> CheckCustomerExistsAsync(Guid idCustomer)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Persons
                .AnyAsync(p => p.Id == idCustomer && p.Discriminator == PersonType.Customer);
        }

        public async Task<Person?> GetCustomerByEmailAsync(string email)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Persons
                .FirstOrDefaultAsync(p => p.Email == email && p.Discriminator == PersonType.Customer);
        }

        public async Task<int> GetTotalCustomersAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Persons
                .CountAsync(p => p.Discriminator == PersonType.Customer);
        }

        public async Task<bool> SoftDeleteCustomerAsync(Guid idCustomer)
        {
            await using var context = _contextFactory.CreateDbContext();
            var customer = await context.Persons
                .FirstOrDefaultAsync(p => p.Id == idCustomer && p.Discriminator == PersonType.Customer);

            if (customer == null)
                return false;

            customer.IsDeleted = true;
            customer.DeletedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreCustomerAsync(Guid idCustomer)
        {
            await using var context = _contextFactory.CreateDbContext();
            var customer = await context.Persons
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == idCustomer && p.Discriminator == PersonType.Customer);

            if (customer == null || !customer.IsDeleted)
                return false;

            customer.IsDeleted = false;
            customer.DeletedAt = null;

            await context.SaveChangesAsync();
            return true;
        }
    }
}