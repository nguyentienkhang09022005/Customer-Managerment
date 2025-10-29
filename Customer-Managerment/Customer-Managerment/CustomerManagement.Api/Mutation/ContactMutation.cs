using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class ContactMutation
    {
        private readonly ContactHandler _contactHandler;

        public ContactMutation(ContactHandler contactHandler)
        {
            _contactHandler = contactHandler;
        }

        public async Task<ContactResponse> CreateContactAsync(ContactCreationRequest contactCreationRequest)
        {
            return await _contactHandler.CreateContactAsync(contactCreationRequest);
        }

        public async Task<string> DeleteContactAsync(Guid idContact)
        {
            return await _contactHandler.DeleteContactAsync(idContact);
        }

        public async Task<ContactResponse> UpdateContactAsync(ContactUpdateRequest contactUpdateRequest, Guid idContact)
        {
            return await _contactHandler.UpdateContactAsync(contactUpdateRequest, idContact);
        }
    }
}
