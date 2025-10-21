using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public ProductRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<List<Product>> GetListProductsAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Products
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .ToListAsync();
        }

        public async Task<Product> AddProductAsync(ProductDomain productDomain)
        {
            await using var context = _contextFactory.CreateDbContext();
            var product = _mapper.Map<Product>(productDomain);
            product.IdProduct = Guid.NewGuid();
            product.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> GetProductByIdAsync(Guid idProduct)
        {
            await using var context = _contextFactory.CreateDbContext();
            var product = await context.Products.AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdProduct == idProduct);

            if (product == null)
                throw new NotFoundException("Không tìm thấy sản phẩm!");

            return product;
        }

        public async Task<Product> UpdateProductAsync(ProductDomain productDomain, Product product)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.Attach(product);

            _mapper.Map(productDomain, product);

            context.Entry(product).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteProductAsync(Guid idProduct)
        {
            await using var context = _contextFactory.CreateDbContext();
            var product = await context.Products.FindAsync(idProduct);
            if (product == null)
                throw new NotFoundException("Không tìm thấy sản phẩm!");

            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckProductExistsAsync(Guid idProduct)
        {
            await using var context = _contextFactory.CreateDbContext();

            return await context.Products
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .AnyAsync(p => p.IdProduct == idProduct);
        }

        public async Task<Product> GetExistingProductAsync(Guid idProduct)
        {
            await using var context = _contextFactory.CreateDbContext();

            var product = await context.Products
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .FirstOrDefaultAsync(p => p.IdProduct == idProduct);

            return product;
        }
    }
}
