using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using HotChocolate.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class ProductMutation
    {
        private readonly ProductHandler _handler;

        public ProductMutation(ProductHandler handler)
        {
            _handler = handler;
        }

        [Authorize]
        public async Task<ProductResponse> CreateProductAsync(ProductCreationRequest request)
        {
            return await _handler.CreateProductAsync(request);
        }

        [Authorize]
        public async Task<ProductResponse> UpdateProductAsync(ProductUpdateRequest request, Guid idProduct)
        {
            return await _handler.UpdateProductAsync(request, idProduct);
        }

        [Authorize]
        public async Task<string> DeleteProductAsync(Guid idProduct)
        {
            return await _handler.DeleteProductAsync(idProduct);
        }
    }
}
