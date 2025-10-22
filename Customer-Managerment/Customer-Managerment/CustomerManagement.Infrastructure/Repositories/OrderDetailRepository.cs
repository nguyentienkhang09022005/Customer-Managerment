using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public OrderDetailRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task AddOrderDetailsAsync(IEnumerable<OrderDetailDomain> orderDetailsDomain, Guid idOrder)
        {
            await using var context = _contextFactory.CreateDbContext();

            var orderDetails = _mapper.Map<List<OrderDetail>>(orderDetailsDomain);

            // Gán IdOrder cho từng chi tiết
            foreach (var detail in orderDetails)
            {
                detail.IdOrder = idOrder;
                detail.IdOrderDetail = Guid.NewGuid();
            }

            await context.OrderDetails.AddRangeAsync(orderDetails);
            await context.SaveChangesAsync();
        }

        public async Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(Guid idOrder)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.OrderDetails
                .Include(d => d.IdProductNavigation)
                .Where(d => d.IdOrder == idOrder)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
