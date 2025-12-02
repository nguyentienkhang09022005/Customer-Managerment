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
    public class ContactRepository : IContactRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public ContactRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, 
                                 IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<ContactDomain> AddContactAsync(ContactDomain contactDomain)
        {   
            await using var context = _contextFactory.CreateDbContext();
            var contact = _mapper.Map<Contact>(contactDomain);
            
            contact.IdContact = Guid.NewGuid();
            contact.Status = StatusContactConstant.ContactPending;
            contact.CreatedAt = DateTime.Now;

            await context.Contacts.AddAsync(contact);
            await context.SaveChangesAsync();
            return _mapper.Map<ContactDomain>(contact);
        }

        public async Task<bool> CheckContactExistsAsync(Guid idContact)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Contacts
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .AnyAsync(s => s.IdContact == idContact);
        }

        public async Task DeleteContactAsync(Guid idContact)
        {
            await using var context = _contextFactory.CreateDbContext();
            var contact = await context.Contacts.FindAsync(idContact);
            if (contact == null)
                throw new NotFoundException("Không tìm thấy hoạt động!");

            context.Contacts.Remove(contact);
            await context.SaveChangesAsync();
        }

        public async Task<ContactDomain?> GetContactByIdAsync(Guid idContact)
        {
            await using var context = _contextFactory.CreateDbContext();
            var contact = await context.Contacts
                    .AsNoTracking()
                    .IgnoreAutoIncludes()
                    .FirstOrDefaultAsync(c => c.IdContact == idContact);

            if (contact == null)
                throw new NotFoundException("Không tìm thấy hoạt động!");
            return _mapper.Map<ContactDomain>(contact);
        }

        public IQueryable<ContactDomain> GetListContact()
        {
            var context = _contextFactory.CreateDbContext();
            return context.Contacts
                .Include(c => c.IdLeadNavigation)
                .Include(c => c.IdStaffNavigation)
                .ProjectTo<ContactDomain>(_mapper.ConfigurationProvider)
                .AsNoTracking();
        }

        public IQueryable<ContactDomain> GetContactById(Guid idContact)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Contacts
                .Where(c => c.IdContact == idContact)
                .Include(c => c.IdLeadNavigation)
                .Include(c => c.IdStaffNavigation)
                .ProjectTo<ContactDomain>(_mapper.ConfigurationProvider)
                .AsNoTracking();
        }

        public async Task<ContactDomain?> UpdateContactAsync(ContactDomain contactDomain)
        {
            await using var context = _contextFactory.CreateDbContext();
            var contact = await context.Contacts.FindAsync(contactDomain.IdContact);
            if (contact == null) return null;

            // Nếu status là Success thì thêm thông tin từ lead sang customer
            if (contactDomain.Status == StatusContactConstant.ContactDone)
            {
                // Lấy thông tin lead
                var lead = await context.Leads
                    .Include(l => l.IdLeadNavigation)
                    .FirstOrDefaultAsync(l => l.IdLead == contactDomain.IdLead);

                if (lead != null)
                {
                    var existCustomer = await context.Customers
                        .FirstOrDefaultAsync(c => c.IdCustomer == lead.IdLead);

                    if (existCustomer == null)
                    {
                        var customer = new Customer
                        {
                            IdCustomer = lead.IdLead,
                            CreatedAt = DateTime.Now,
                            IdCustomerNavigation = lead.IdLeadNavigation
                        };

                        await context.Customers.AddAsync(customer);
                        await context.SaveChangesAsync();
                    }
                }
            }    

            // Cập nhật các thuộc tính của contact
            _mapper.Map(contactDomain, contact);
            context.Entry(contact).Property(c => c.Type).IsModified = false;
            context.Entry(contact).Property(c => c.Title).IsModified = false;
            context.Entry(contact).Property(c => c.Content).IsModified = false;

            await context.SaveChangesAsync();
            return _mapper.Map<ContactDomain>(contact);
        }

        public async Task<int> getTotalContactsAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Contacts.CountAsync();
        }

        public async Task<QuantityStatisticsDetailContactResponse> QuantityStatisticsDetailContactResponse()
        {
            await using var context = _contextFactory.CreateDbContext();
            var totalPending = await context.Contacts
                .AsNoTracking()
                .CountAsync(c => c.Status == StatusContactConstant.ContactPending);
            var totalInProgress = await context.Contacts
                .AsNoTracking()
                .CountAsync(c => c.Status == StatusContactConstant.ContactInProgress);
            var totalFailed = await context.Contacts
                .AsNoTracking()
                .CountAsync(c => c.Status == StatusContactConstant.ContactFailed);
            var totalDone = await context.Contacts
                .AsNoTracking()
                .CountAsync(c => c.Status == StatusContactConstant.ContactDone);
            var totalCanceled = await context.Contacts
                .AsNoTracking()
                .CountAsync(c => c.Status == StatusContactConstant.ContactCanceled);
            return new QuantityStatisticsDetailContactResponse
            {
                QuantityContactsPending = totalPending,
                QuantityContactsInProgress = totalInProgress,
                QuantityContactsDone = totalDone,
                QuantityContactsCancel = totalCanceled,
                QuantityContactsFailed = totalFailed
            };
        }
    }
}
