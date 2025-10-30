using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class CustomerMutation
    {
        private readonly CustomerHanler _customerHanler;

        public CustomerMutation(CustomerHanler customerHanler)
        {
            _customerHanler = customerHanler;
        }

        public async Task<CustomerResponse> CreateCustomerAsync(CustomerCreationRequest customerCreationRequest)
        {
            return await _customerHanler.CreateCustomerAsync(customerCreationRequest);
        }

        public async Task<string> DeleteCustomerAsync(Guid idCustomer)
        {
            return await _customerHanler.DeletecCustomerAsync(idCustomer);
        }

        public async Task<CustomerResponse> UpdateCustomerAsync(CustomerUpdateRequest customerUpdateRequest, Guid idCustomer)
        {
            return await _customerHanler.UpdateCustomerAsync(customerUpdateRequest, idCustomer);
        }
    }
}
