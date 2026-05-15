using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    // [ExtendObjectType(OperationTypeNames.Query)]
    // public class DealsElasticSearchQuery
    // {
    //     private readonly IElasticsearchService _elasticsearchService;
    //     public DealsElasticSearchQuery(IElasticsearchService elasticsearchService)
    //     {
    //         _elasticsearchService = elasticsearchService;
    //     }

    //     public async Task<List<DealResponse>> SearchDealsAsync(string keyword)
    //     {
    //         return await _elasticsearchService.SearchAsync<DealResponse>(keyword,
    //                                                                      "deals",
    //                                                                      "title",
    //                                                                      "content",
    //                                                                      "price",
    //                                                                      "status");
    //     }
    // }
}
