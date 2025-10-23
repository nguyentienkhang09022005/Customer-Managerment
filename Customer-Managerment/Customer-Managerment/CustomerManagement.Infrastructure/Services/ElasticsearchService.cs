using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Nest;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Services
{
    public class ElasticsearchService : IElasticsearchService
    {
        private readonly IElasticClient _client;
        private const string UserIndexName = "users";
        private readonly IConfiguration _config;
        public ElasticsearchService(IConfiguration config)
        {
            _config = config;

            var uriString = _config["Elasticsearch:Uri"];
            if (string.IsNullOrEmpty(uriString))
                throw new Exception("Elasticsearch URI not configured!");

            var uri = new Uri(uriString);
            var settings = new ConnectionSettings(uri)
                .DefaultIndex(UserIndexName); 

            _client = new ElasticClient(settings);

            // Tạo index nếu chưa có
            _client.Indices.Create(UserIndexName, c => c
                .Map<UserResponse>(m => m.AutoMap())
            );
        }

        public async Task IndexUserAsync(UserResponse user)
        {
            // ES tự động tạo mới nếu chưa có ID, hoặc cập nhật nếu ID đã tồn tại
            await _client.IndexDocumentAsync(user);
        }

        public async Task DeleteUserAsync(Guid idUser)
        {
            // Xóa document dựa trên ID
            await _client.DeleteAsync<UserResponse>(idUser.ToString());
        }

        public async Task<List<UserResponse>> SearchUsersAsync(string keyword)
        {
            var response = await _client.SearchAsync<UserResponse>(s => s
                .Query(q => q
                    .MultiMatch(mm => mm // Tìm kiếm trên nhiều trường
                        .Query(keyword)
                        .Fields(f => f
                            .Fields(u => u.IdUser)
                            .Field(u => u.Fullname)
                            .Field(u => u.Email)
                            .Field(u => u.Phone)
                            .Field(u => u.Address)
                        )
                        .Fuzziness(Fuzziness.Auto) // Cho phép tìm kiếm "gần đúng"
                    )
                )
            );

            return response.Documents.ToList();
        }
    }
}