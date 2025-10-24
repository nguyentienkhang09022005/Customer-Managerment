using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IContactRepository
    {
        Task<Contact> AddContactAsync(ContactDomain contactDomain);

        Task DeleteContactAsync(Guid idContact);

        Task<Contact?> UpdateContactAsync(ContactDomain contactDomain);

        Task<ContactDomain?> GetContactByIdAsync(Guid idContact);

        Task<List<Contact>> GetListContactAsync();


        Task<bool> CheckContactExistsAsync(Guid idContact);

        Task<Contact?> GetExistingContactAsync(Guid idContact);
    }
}
