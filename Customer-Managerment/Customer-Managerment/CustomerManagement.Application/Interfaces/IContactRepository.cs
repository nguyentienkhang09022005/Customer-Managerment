using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IContactRepository
    {
        Task<ContactDomain> AddContactAsync(ContactDomain contactDomain);

        Task DeleteContactAsync(Guid idContact);

        Task<ContactDomain?> UpdateContactAsync(ContactDomain contactDomain);

        Task<ContactDomain?> GetContactByIdAsync(Guid idContact);

        Task<List<ContactDomain>> GetListContactAsync();

        Task<bool> CheckContactExistsAsync(Guid idContact);
    }
}
