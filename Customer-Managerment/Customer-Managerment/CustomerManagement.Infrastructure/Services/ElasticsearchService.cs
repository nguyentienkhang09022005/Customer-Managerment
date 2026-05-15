// // Elasticsearch Service - Commented out for future development
// using Customer_Managerment.CustomerManagement.Application.Interfaces;
// using Nest;

// namespace Customer_Managerment.CustomerManagement.Infrastructure.Services
// {
//     public class ElasticsearchService : IElasticsearchService
//     {
//         private readonly IElasticClient _client;
//         private readonly IConfiguration _config;

//         public ElasticsearchService(IConfiguration config)
//         {
//             _config = config;

//             var uriString = _config["Elasticsearch:Uri"];
//             if (string.IsNullOrEmpty(uriString))
//                 throw new Exception("Elasticsearch URI not configured!");

//             var uri = new Uri(uriString);
//             var settings = new ConnectionSettings(uri)
//                     .DisableDirectStreaming();

//             _client = new ElasticClient(settings);
//         }

//         public async Task IndexAsync<T>(T document, string index) where T : class
//         {
//             await CreateIndexIfNotExist<T>(index);

//             var idProp = document.GetType().GetProperty("IdLead")
//                          ?? document.GetType().GetProperty("IdCustomer")
//                          ?? document.GetType().GetProperty("IdDeal")
//                          ?? document.GetType().GetProperty("IdContact")
//                          ?? document.GetType().GetProperty("Id");
//             var idValue = idProp?.GetValue(document)?.ToString();

//             var response = await _client.IndexAsync(document, idx => idx
//                 .Index(index)
//                 .Id(idValue)
//             );
//         }

//         public async Task DeleteAsync<T>(string id, string index) where T : class
//         {
//             await _client.DeleteAsync<T>(id, del => del.Index(index));
//         }

//         public async Task<List<T>> SearchAsync<T>(string keyword, string indexName, params string[] fields) where T : class
//         {
//             await CreateIndexIfNotExist<T>(indexName);

//             var response = await _client.SearchAsync<T>(s => s
//                 .Index(indexName)
//                 .Query(q => q
//                     .MultiMatch(mm => mm
//                         .Query(keyword)
//                         .Fields(f => f.Fields(fields))
//                         .Fuzziness(Fuzziness.Auto)
//                     )
//                 )
//             );

//             return response.Documents.ToList();
//         }

//         private async Task CreateIndexIfNotExist<T>(string indexName) where T : class
//         {
//             var exists = await _client.Indices.ExistsAsync(indexName);
//             if (exists.Exists) return;

//             await _client.Indices.CreateAsync(indexName, c => c.Map<T>(m => m.AutoMap()));
//         }
//     }
// }