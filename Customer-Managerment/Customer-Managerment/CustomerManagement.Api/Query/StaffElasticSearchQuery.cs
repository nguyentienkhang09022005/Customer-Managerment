using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    // [ExtendObjectType(OperationTypeNames.Query)]
    // public class StaffElasticSearchQuery
    // {
    //     private readonly IElasticsearchService _elasticsearchService;
    //     public StaffElasticSearchQuery(IElasticsearchService elasticsearchService)
    //     {
    //         _elasticsearchService = elasticsearchService;
    //     }

    //     public async Task<List<StaffResponse>> SearchStaffsAsync(string keyword)
    //     {
    //         return await _elasticsearchService.SearchAsync<StaffResponse>(keyword,
    //                                                                       "staffs",
    //                                                                       "fullname",
    //                                                                       "email");
    //     }
    // }
}
