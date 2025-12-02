using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IDealRepository
    {
        Task<DealDomain> AddDealAsync(DealDomain dealDomain);

        Task DeleteDealAsync(Guid idDeal);

        Task<DealDomain?> UpdateDealAsync(DealDomain dealDomain);

        Task<DealDomain?> GetDealByIdAsync(Guid idDeal);

        IQueryable<DealDomain> GetListDeal();

        IQueryable<DealDomain> GetDealById(Guid idDeal);

        Task<bool> CheckDealExistsAsync(Guid idDeal);

        Task<decimal> GetTotalProfitAsync();

        Task<int> getTotalDealsAsync();

        Task<QuantityStatisticsDetailDealResponse> QuantityStatisticsDetailDealResponse();
    }
}
