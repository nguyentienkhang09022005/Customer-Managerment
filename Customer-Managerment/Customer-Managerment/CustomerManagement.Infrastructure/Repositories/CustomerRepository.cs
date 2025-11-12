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
    public class CustomerRepository : ICustomerRepository
    { 
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public CustomerRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<CustomerDomain> AddCustomerAsync(CustomerDomain customerDomain)
        {
            await using var context = _contextFactory.CreateDbContext();

            if (customerDomain.personDomain == null)
                throw new BadRequestException("Thông tin Person không hợp lệ!");

            var sharedId = Guid.NewGuid();

            var personEntity = _mapper.Map<Person>(customerDomain.personDomain);
            personEntity.IdPerson = sharedId;
            await context.Person.AddAsync(personEntity);
            await context.SaveChangesAsync();

            var customerEntity = new Customer
            {
                IdCustomer = sharedId,
                CreatedAt = DateTime.Now,
                IdCustomerNavigation = personEntity
            };

            await context.Customers.AddAsync(customerEntity);
            await context.SaveChangesAsync();

            return _mapper.Map<CustomerDomain>(customerEntity);
        }

        public async Task<bool> CheckCustomerExistsAsync(Guid idCustomer)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Customers.AnyAsync(c => c.IdCustomer == idCustomer);
        }

        public async Task DeleteCustomerAsync(Guid idCustomer)
        {
            await using var context = _contextFactory.CreateDbContext();
            var customer = await context.Customers
                .Include(c => c.IdCustomerNavigation)
                .FirstOrDefaultAsync(c => c.IdCustomer == idCustomer);

            if (customer == null) throw new NotFoundException("Không tìm thấy Customer!");

            context.Person.Remove(customer.IdCustomerNavigation);
            context.Customers.Remove(customer);
            await context.SaveChangesAsync();
        }

        public IQueryable<CustomerDomain> GetCustomerById(Guid idCustomer)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Customers
                .Where(c => c.IdCustomer == idCustomer)
                .Include(c => c.IdCustomerNavigation)
                .ProjectTo<CustomerDomain>(_mapper.ConfigurationProvider)
                .AsNoTracking();
        }

        public async Task<CustomerDomain?> GetCustomerByIdAsync(Guid idCustomer)
        {
            await using var context = _contextFactory.CreateDbContext();
            var customer = await context.Customers
                .Include(c => c.IdCustomerNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdCustomer == idCustomer);

            if (customer == null) throw new NotFoundException("Không tìm thấy Customer!");
            return _mapper.Map<CustomerDomain>(customer);
        }

        public IQueryable<CustomerDomain> GetListCustomer()
        {
            var context = _contextFactory.CreateDbContext();
            return context.Customers
                .Include(c => c.IdCustomerNavigation)
                .ProjectTo<CustomerDomain>(_mapper.ConfigurationProvider)
                .AsNoTracking();
        }

        public async Task<CustomerDomain?> UpdateCustomerAsync(CustomerDomain customerDomain)
        {
            await using var context = _contextFactory.CreateDbContext();
            var existingCustomer = await context.Customers
                .Include(c => c.IdCustomerNavigation)
                .FirstOrDefaultAsync(c => c.IdCustomer == customerDomain.IdCustomer);

            if (existingCustomer == null) return null;

            _mapper.Map(customerDomain, existingCustomer);

            if (customerDomain.personDomain != null && existingCustomer.IdCustomerNavigation != null)
            {
                _mapper.Map(customerDomain.personDomain, existingCustomer.IdCustomerNavigation);
            }

            await context.SaveChangesAsync();
            return _mapper.Map<CustomerDomain>(existingCustomer);
        }
    }
}
