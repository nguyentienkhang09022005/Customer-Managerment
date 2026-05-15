using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    // [ExtendObjectType(OperationTypeNames.Query)]
    // public class ContactsElasticSearchQuery
    // {
    //     private readonly IElasticsearchService _elasticsearchService;
    //     public ContactsElasticSearchQuery(IElasticsearchService elasticsearchService)
    //     {
    //         _elasticsearchService = elasticsearchService;
    //     }

    //     public async Task<List<ContactResponse>> SearchContactsAsync(string keyword)
    //     {
    //         return await _elasticsearchService.SearchAsync<ContactResponse>(keyword,
    //                                                                          "contacts",
    //                                                                          "type",
    //                                                                          "title",
    //                                                                          "content",
    //                                                                          "status");
    //     }
    // }
}
