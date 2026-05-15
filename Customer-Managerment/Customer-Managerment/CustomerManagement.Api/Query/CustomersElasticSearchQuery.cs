using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    // [ExtendObjectType(OperationTypeNames.Query)]
    // public class CustomersElasticSearchQuery
    // {
    //     private readonly IElasticsearchService _elasticsearchService;
    //     public CustomersElasticSearchQuery(IElasticsearchService elasticsearchService)
    //     {
    //         _elasticsearchService = elasticsearchService;
    //     }

    //     public async Task<List<CustomerResponse>> SearchCustomersAsync(string keyword)
    //     {
    //         return await _elasticsearchService.SearchAsync<CustomerResponse>(keyword,
    //                                                                          "customers",
    //                                                                          "personResponse.fullname",
    //                                                                          "personResponse.email",
    //                                                                          "personResponse.phone",
    //                                                                          "personResponse.location");
    //     }
    // }
}
