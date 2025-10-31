using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class ElasticSearchQuery
    {
        private readonly IElasticsearchService _elasticsearchService;
        public ElasticSearchQuery(IElasticsearchService elasticsearchService)
        {
            _elasticsearchService = elasticsearchService;
        }

        public async Task<List<StaffResponse>> SearchStaffsAsync(string keyword)
        {
            return await _elasticsearchService.SearchStaffsAsync(keyword);
        }
    }
}
