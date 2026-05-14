using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IContactRepository
    {
        Task<Contact> AddContactAsync(Contact contact);
        Task<Contact?> GetContactByIdAsync(Guid idContact);
        IQueryable<Contact> GetListContact();
        Task<List<Contact>> GetListContactAsync();
        IQueryable<Contact> GetContactById(Guid idContact);
        Task<Contact?> UpdateContactAsync(Contact contact);
        Task<bool> CheckContactExistsAsync(Guid idContact);
        Task<int> GetTotalContactsAsync();
        Task<QuantityStatisticsDetailContactResponse> QuantityStatisticsDetailContactResponse();
        Task<bool> SoftDeleteContactAsync(Guid idContact);
    }
}