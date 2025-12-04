using Customer_Managerment.CustomerManagement.Application.DTOs.Response;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IElasticsearchService
    {
        Task IndexAsync<T>(T document, string index) where T : class;

        Task DeleteAsync<T>(string id, string index) where T : class;

        Task<List<T>> SearchAsync<T>(string keyword, string indexName, params string[] fields) where T : class;
    }
}