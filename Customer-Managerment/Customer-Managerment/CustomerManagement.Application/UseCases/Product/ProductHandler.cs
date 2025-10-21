using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Application.UseCases.Product
{
    public class ProductHandler
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<ProductResponse>> GetListProductsAsync()
        {
            var list = await _productRepository.GetListProductsAsync();
            return _mapper.Map<List<ProductResponse>>(list);
        }

        public async Task<ProductResponse> GetInfProductAsync(Guid idProduct)
        {
            var product = await _productRepository.GetProductByIdAsync(idProduct);
            return _mapper.Map<ProductResponse>(product);
        }

        public async Task<ProductResponse> CreateProductAsync(ProductCreationRequest request)
        {
            var domain = _mapper.Map<ProductDomain>(request);
            var newProduct = await _productRepository.AddProductAsync(domain);
            return _mapper.Map<ProductResponse>(newProduct);
        }

        public async Task<ProductResponse> UpdateProductAsync(ProductUpdateRequest productUpdateRequest, Guid idProduct)
        {
            var productEntity = await _productRepository.GetExistingProductAsync(idProduct);
            if (productEntity == null)
            {
                throw new DomainException("Không tìm thấy sản phẩm cần đổi thông tin", 404);
            }
            var productDomain = _mapper.Map<ProductDomain>(productEntity);

            _mapper.Map(productUpdateRequest, productDomain);

            var updatedProduct = await _productRepository.UpdateProductAsync(productDomain, productEntity);

            return _mapper.Map<ProductResponse>(updatedProduct);
        }

        public async Task<string> DeleteProductAsync(Guid idProduct)
        {
            await _productRepository.DeleteProductAsync(idProduct);
            return "Xóa sản phẩm thành công!";
        }
    }
}
