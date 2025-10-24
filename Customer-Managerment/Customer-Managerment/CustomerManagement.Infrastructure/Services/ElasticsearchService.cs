using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Nest;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Services
{
    public class ElasticsearchService : IElasticsearchService
    {
        private readonly IElasticClient _client;
        private const string StaffIndexName = "staffs";
        private readonly IConfiguration _config;
        public ElasticsearchService(IConfiguration config)
        {
            _config = config;

            var uriString = _config["Elasticsearch:Uri"];
            if (string.IsNullOrEmpty(uriString))
                throw new Exception("Elasticsearch URI not configured!");

            var uri = new Uri(uriString);
            var settings = new ConnectionSettings(uri)
                .DefaultIndex(StaffIndexName); 

            _client = new ElasticClient(settings);

            // Tạo index nếu chưa có
            _client.Indices.Create(StaffIndexName, c => c
                .Map<StaffResponse>(m => m.AutoMap())
            );
        }

        public async Task IndexStaffAsync(StaffResponse staffResponse)
        {
            // ES tự động tạo mới nếu chưa có ID, hoặc cập nhật nếu ID đã tồn tại
            await _client.IndexDocumentAsync(staffResponse);
        }

        public async Task DeleteStaffAsync(Guid idStaff)
        {
            // Xóa document dựa trên ID
            await _client.DeleteAsync<StaffResponse>(idStaff.ToString());
        }

        public async Task<List<StaffResponse>> SearchStaffsAsync(string keyword)
        {
            var response = await _client.SearchAsync<StaffResponse>(s => s
                .Query(q => q
                    .MultiMatch(mm => mm // Tìm kiếm trên nhiều trường
                        .Query(keyword)
                        .Fields(f => f
                            .Fields(u => u.IdStaff)
                            .Field(u => u.Fullname)
                            .Field(u => u.Email)
                        )
                        .Fuzziness(Fuzziness.Auto) // Cho phép tìm kiếm "gần đúng"
                    )
                )
            );

            return response.Documents.ToList();
        }
    }
}