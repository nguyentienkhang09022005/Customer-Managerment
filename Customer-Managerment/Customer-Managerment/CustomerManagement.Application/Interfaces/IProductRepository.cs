using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetListProductsAsync();

        Task<Product> AddProductAsync(ProductDomain productDomain);

        Task<Product> GetProductByIdAsync(Guid idProduct);

        Task<Product> UpdateProductAsync(ProductDomain productDomain, Product product);

        Task DeleteProductAsync(Guid idProduct);

        Task<bool> CheckProductExistsAsync(Guid idProduct);

        Task<Product> GetExistingProductAsync(Guid idProduct);
    }
}
