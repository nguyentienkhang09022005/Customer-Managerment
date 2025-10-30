using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Services
{
    public class ChatHistoryService : IChatHistoryService
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _jsonOptions;

        public ChatHistoryService(IDistributedCache cache)
        {
            _cache = cache;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        private string GetCacheKey(Guid idStaff) => $"chat_history:{idStaff}";

        public async Task DeleteHistoryAsync(Guid idStaff)
        {
            await _cache.RemoveAsync(GetCacheKey(idStaff));
        }

        public async Task<List<MessageHistoryItem>> GetHistoryAsync(Guid idStaff)
        {
            var key = GetCacheKey(idStaff);
            var jsonData = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(jsonData))
                return new List<MessageHistoryItem>();

            return JsonSerializer.Deserialize<List<MessageHistoryItem>>(jsonData, _jsonOptions)!;
        }

        public async Task SaveMessageAsync(Guid idStaff, MessageHistoryItem message)
        {
            var key = GetCacheKey(idStaff);
            var existingData = await _cache.GetStringAsync(key);

            List<MessageHistoryItem> history;
            if (!string.IsNullOrEmpty(existingData))
            {
                history = JsonSerializer.Deserialize<List<MessageHistoryItem>>(existingData, _jsonOptions)!;
            }
            else
            {
                history = new List<MessageHistoryItem>();
            }

            history.Add(message);

            var jsonData = JsonSerializer.Serialize(history, _jsonOptions);

            await _cache.SetStringAsync(
                key,
                jsonData,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                });
        }
    }
}
