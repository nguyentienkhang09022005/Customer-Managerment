using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases.OrderHandler;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class OrderQuery
    {
        private readonly OrderHandler _handler;

        public OrderQuery(OrderHandler handler)
        {
            _handler = handler;
        }

        [UseProjection]        //[Authorize]

        [UseFiltering]
        [UseSorting]
        public async Task<List<OrderResponse>> GetOrdersAsync(Guid idUser)
        {
            return await _handler.GetListOrdersAsync(idUser);
        }

        //[Authorize]
        public async Task<OrderResponse> GetInfOrderAsync(Guid idOrder)
        {
            return await _handler.GetInfOrderAsync(idOrder);
        }
    }
}
