using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class LeadsElasticSearchQuery
    {
        private readonly IElasticsearchService _elasticsearchService;
        public LeadsElasticSearchQuery(IElasticsearchService elasticsearchService)
        {
            _elasticsearchService = elasticsearchService;
        }

        public async Task<List<LeadResponse>> SearchLeadsAsync(string keyword)
        {
            return await _elasticsearchService.SearchAsync<LeadResponse>(keyword,
                                                                        "leads",
                                                                        "resource",
                                                                        "personResponse.fullname",
                                                                        "personResponse.email",
                                                                        "personResponse.phone",
                                                                        "personResponse.location");

        }
    }
}
