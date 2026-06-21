using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Domain.Entities;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IDealRepository
    {
        Task<Deal> AddDealAsync(Deal deal);
        Task<Deal?> GetDealByIdAsync(Guid idDeal);
        IQueryable<Deal> GetListDeal();
        Task<List<Deal>> GetListDealAsync();
        Task<(List<Deal> Items, int TotalCount)> GetListDealPagedAsync(int page, int pageSize);
        IQueryable<Deal> GetDealById(Guid idDeal);
        Task<Deal?> UpdateDealAsync(Deal deal);
        Task<bool> CheckDealExistsAsync(Guid idDeal);
        Task<decimal> GetTotalProfitAsync();
        Task<int> GetTotalDealsAsync();
        Task<QuantityStatisticsDetailDealResponse> QuantityStatisticsDetailDealResponse();
        Task<bool> SoftDeleteDealAsync(Guid idDeal);
    }
}