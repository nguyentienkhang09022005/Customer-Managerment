using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class ProductQuery
    {
        private readonly ProductHandler _handler;

        public ProductQuery(ProductHandler handler)
        {
            _handler = handler;
        }

        [Authorize]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public async Task<List<ProductResponse>> GetProductsAsync()
        {
            return await _handler.GetListProductsAsync();
        }

        [Authorize]
        public async Task<ProductResponse> GetInfProductAsync(Guid idProduct)
        {
            return await _handler.GetInfProductAsync(idProduct);
        }
    }
}
