using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class OrderMutation
    {
        private readonly OrderHandler _handler;

        public OrderMutation(OrderHandler handler)
        {
            _handler = handler;
        }

        [Authorize]
        public async Task<string> CreateOrderAsync(OrderCreationRequest request)
        {
            return await _handler.CreateOrderAsync(request);
        }

        [Authorize]
        public async Task<string> DeleteOrderAsync(Guid idOrder)
        {
            return await _handler.DeleteOrderAsync(idOrder);
        }
    }
}
