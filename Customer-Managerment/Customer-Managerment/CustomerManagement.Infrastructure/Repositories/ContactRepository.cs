using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private const int MaxUnboundedRecords = 1000;

        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactRepository> _logger;

        public ContactRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory,
                                 IMapper mapper,
                                 ILogger<ContactRepository> logger)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Contact> AddContactAsync(Contact contact)
        {
            await using var context = _contextFactory.CreateDbContext();

            contact.IdContact = Guid.NewGuid();
            contact.Status = StatusContactConstant.ContactNew;
            contact.CreatedAt = DateTime.UtcNow;
            contact.IsDeleted = false;

            await context.Contacts.AddAsync(contact);
            await context.SaveChangesAsync();
            return contact;
        }

        public async Task<Contact?> GetContactByIdAsync(Guid idContact)
        {
            await using var context = _contextFactory.CreateDbContext();
            var contact = await context.Contacts
                    .IgnoreQueryFilters()
                    .Include(c => c.IdLeadNavigation)
                    .Include(c => c.IdStaffNavigation)
                    .FirstOrDefaultAsync(c => c.IdContact == idContact);

            if (contact == null)
                throw new NotFoundException("Không tìm thấy hoạt động!");

            return contact;
        }

        public IQueryable<Contact> GetListContact()
        {
            var context = _contextFactory.CreateDbContext();
            return context.Contacts
                .Include(c => c.IdLeadNavigation)
                .Include(c => c.IdStaffNavigation)
                .AsNoTracking();
        }

        public async Task<List<Contact>> GetListContactAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            var total = await context.Contacts.CountAsync();
            if (total > MaxUnboundedRecords)
            {
                _logger.LogWarning("GetListContactAsync returned {Returned}/{Total} records (hard cap {Cap}). Use GetListContactPagedAsync for full data.",
                    MaxUnboundedRecords, total, MaxUnboundedRecords);
            }
            return await context.Contacts
                .Include(c => c.IdLeadNavigation)
                .Include(c => c.IdStaffNavigation)
                .AsNoTracking()
                .Take(MaxUnboundedRecords)
                .ToListAsync();
        }

        public async Task<(List<Contact> Items, int TotalCount)> GetListContactPagedAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 200) pageSize = 200;

            await using var context = _contextFactory.CreateDbContext();
            var total = await context.Contacts.CountAsync();
            var items = await context.Contacts
                .Include(c => c.IdLeadNavigation)
                .Include(c => c.IdStaffNavigation)
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, total);
        }

        public IQueryable<Contact> GetContactById(Guid idContact)
        {
            var context = _contextFactory.CreateDbContext();
            return context.Contacts
                .Where(c => c.IdContact == idContact)
                .Include(c => c.IdLeadNavigation)
                .Include(c => c.IdStaffNavigation)
                .AsNoTracking();
        }

        public async Task<Contact?> UpdateContactAsync(Contact contact)
        {
            await using var context = _contextFactory.CreateDbContext();
            var existingContact = await context.Contacts
                .Include(c => c.IdLeadNavigation)
                .Include(c => c.IdStaffNavigation)
                .FirstOrDefaultAsync(c => c.IdContact == contact.IdContact);

            if (existingContact == null)
                return null;

            existingContact.Type = contact.Type;
            existingContact.Title = contact.Title;
            existingContact.Content = contact.Content;
            existingContact.Status = contact.Status;
            existingContact.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return existingContact;
        }

        public async Task<bool> CheckContactExistsAsync(Guid idContact)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Contacts
                .AnyAsync(c => c.IdContact == idContact);
        }

        public async Task<int> GetTotalContactsAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Contacts.CountAsync();
        }

        public async Task<QuantityStatisticsDetailContactResponse> QuantityStatisticsDetailContactResponse()
        {
            await using var context = _contextFactory.CreateDbContext();
            var totalNew = await context.Contacts
                .CountAsync(c => c.Status == StatusContactConstant.ContactNew);
            var totalInProgress = await context.Contacts
                .CountAsync(c => c.Status == StatusContactConstant.ContactInProgress);
            var totalFailed = await context.Contacts
                .CountAsync(c => c.Status == StatusContactConstant.ContactFailed);
            var totalDone = await context.Contacts
                .CountAsync(c => c.Status == StatusContactConstant.ContactSuccess);
            var totalCanceled = await context.Contacts
                .CountAsync(c => c.Status == StatusContactConstant.ContactCanceled);

            return new QuantityStatisticsDetailContactResponse
            {
                QuantityContactsPending = totalNew,
                QuantityContactsInProgress = totalInProgress,
                QuantityContactsDone = totalDone,
                QuantityContactsCancel = totalCanceled,
                QuantityContactsFailed = totalFailed
            };
        }

        public async Task<bool> SoftDeleteContactAsync(Guid idContact)
        {
            await using var context = _contextFactory.CreateDbContext();
            var rows = await context.Contacts
                .Where(c => c.IdContact == idContact)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.IsDeleted, true)
                    .SetProperty(c => c.DeletedAt, DateTime.UtcNow));
            return rows > 0;
        }
    }
}