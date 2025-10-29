using AutoMapper;
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

        public async Task<List<ContactDomain>> GetListContactAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return _mapper.Map<List<ContactDomain>>(
                await context.Contacts
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .ToListAsync());
        }

        public async Task<ContactDomain?> UpdateContactAsync(ContactDomain contactDomain)
        {
            await using var context = _contextFactory.CreateDbContext();
            var contact = await context.Contacts.FindAsync(contactDomain.IdContact);
            if (contact == null) return null;

            // Cập nhật các thuộc tính của staff
            _mapper.Map(contactDomain, contact);
            await context.SaveChangesAsync();
            return _mapper.Map<ContactDomain>(contact);
        }
    }
}
