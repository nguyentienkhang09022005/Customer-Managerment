using System.Net.Http.Headers;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Services
{
    public class GroqService : IGroqService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _groqApiUrlBase;
        private readonly string _model;
        private readonly double _temperature;
        private readonly int _maxTokens;
        private readonly double _topP;
        private readonly int _timeoutSeconds;

        public GroqService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["GroqSettings:ApiKey"]
                      ?? throw new ArgumentNullException("GroqSettings:ApiKey not configured.");
            _groqApiUrlBase = config["GroqSettings:BaseUrl"]
                              ?? throw new ArgumentNullException("GroqSettings:BaseUrl not configured.");
            _model = config["GroqSettings:Model"]
                     ?? "meta-llama/llama-4-scout-17b-16e-instruct";
            _temperature = double.TryParse(config["GroqSettings:Temperature"], out var t) ? t : 0.15;
            _maxTokens = int.TryParse(config["GroqSettings:MaxTokens"], out var m) ? m : 600;
            _topP = double.TryParse(config["GroqSettings:TopP"], out var p) ? p : 0.9;
            _timeoutSeconds = int.TryParse(config["GroqSettings:TimeoutSeconds"], out var s) ? s : 120;

            _httpClient.BaseAddress = new Uri(_groqApiUrlBase);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.Timeout = TimeSpan.FromSeconds(_timeoutSeconds);
        }

        public async Task<string> GenerateChatResponseAsync(string systemInstruction,
                                                            List<MessageHistoryItem> history,
                                                            string userMessage)
        {
            var messages = new List<GroqMessage>();

            if (!string.IsNullOrWhiteSpace(systemInstruction))
            {
                messages.Add(new GroqMessage { Role = "system", Content = systemInstruction });
            }

            foreach (var item in history)
            {
                messages.Add(new GroqMessage
                {
                    Role = NormalizeRole(item.Role),
                    Content = item.Message
                });
            }

            messages.Add(new GroqMessage { Role = "user", Content = userMessage });

            var groqRequest = new GroqRequest
            {
                Model = _model,
                Messages = messages,
                Temperature = _temperature,
                MaxTokens = _maxTokens,
                TopP = _topP
            };

            var fullUrl = $"{_groqApiUrlBase.TrimEnd('/')}/chat/completions";
            var response = await _httpClient.PostAsJsonAsync(fullUrl, groqRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Groq API call failed: {response.StatusCode} - {errorBody}");
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<GroqApiResponse>();

            var generatedText = apiResponse?.Choices?.FirstOrDefault()?.Message?.Content;

            return generatedText ?? "Xin lỗi, tôi không thể tạo phản hồi vào lúc này.";
        }

        public async Task<string> GetWelcomeMessageAsync()
        {
            var aiIntroduction = "Xin chào, tôi là AI tư vấn về bất động sản. Bạn cần tôi hỗ trợ gì hôm nay nào?";

            return aiIntroduction;
        }

        private static string NormalizeRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return "user";

            return role.ToLower() switch
            {
                "model" or "assistant" => "assistant",
                "user" => "user",
                "system" => "system",
                _ => "user"
            };
        }
    }
}
