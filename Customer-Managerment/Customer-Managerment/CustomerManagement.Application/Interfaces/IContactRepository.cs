using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IContactRepository
    {
        Task<ContactDomain> AddContactAsync(ContactDomain contactDomain);

        Task DeleteContactAsync(Guid idContact);

        Task<ContactDomain?> UpdateContactAsync(ContactDomain contactDomain);

        Task<ContactDomain?> GetContactByIdAsync(Guid idContact);

        IQueryable<ContactDomain> GetListContact();

        Task<List<ContactDomain>> GetListContactAsync();

        IQueryable<ContactDomain> GetContactById(Guid idContact);

        Task<bool> CheckContactExistsAsync(Guid idContact);

        Task<int> getTotalContactsAsync();

        Task<QuantityStatisticsDetailContactResponse> QuantityStatisticsDetailContactResponse();
    }
}
