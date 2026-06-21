using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    [Authorize]
    public class ContactMutation
    {
        private readonly ContactHandler _contactHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContactMutation(ContactHandler contactHandler, IHttpContextAccessor httpContextAccessor)
        {
            _contactHandler = contactHandler;
            _httpContextAccessor = httpContextAccessor;
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

        private string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        }
    }
}
