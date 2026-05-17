using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class CustomerMutation
    {
        private readonly CustomerHandler _customerHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerMutation(CustomerHandler customerHandler, IHttpContextAccessor httpContextAccessor)
        {
            _customerHandler = customerHandler;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CustomerResponse> CreateCustomerAsync(CustomerCreationRequest request)
        {
            return await _customerHandler.CreateCustomerAsync(request);
        }

        public async Task<bool> DeleteCustomerAsync(Guid idCustomer)
        {
            await _customerHandler.DeleteCustomerAsync(idCustomer);
            return true;
        }

        public async Task<CustomerResponse> UpdateCustomerAsync(CustomerUpdateRequest request, Guid idCustomer)
        {
            return await _customerHandler.UpdateCustomerAsync(request, idCustomer);
        }

        public async Task<CustomerResponse> RestoreCustomerAsync(Guid idCustomer)
        {
            return await _customerHandler.RestoreCustomerAsync(idCustomer);
        }

        // File upload moved to /api/fileupload/customer

        private string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst("sub")?.Value ?? "system";
        }
    }
}