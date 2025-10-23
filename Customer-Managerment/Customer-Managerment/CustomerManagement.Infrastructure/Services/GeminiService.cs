using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Infrastructure.Services.GeminiModels;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _geminiApiUrlBase;
        public GeminiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["GeminiSettings:ApiKey"]
                      ?? throw new ArgumentNullException("Gemini:ApiKey not configured.");
            _geminiApiUrlBase = config["GeminiSettings:BaseUrl"]
                                ?? throw new ArgumentNullException("Gemini:BaseUrl not configured.");
        }

        public async Task<string> GenerateChatResponseAsync(string systemInstruction,
                                                            List<MessageHistoryItem> history,
                                                            string userMessage)
        {
            var geminiRequest = new GeminiRequest
            {
                // Thêm hướng dẫn hệ thống (chứa thông tin sản phẩm)
                SystemInstruction = new SystemInstruction
                {
                    Parts = new List<GeminiPart> { new GeminiPart { Text = systemInstruction } }
                },
                Contents = new List<GeminiContent>()
            };

            // Thêm lịch sử trò chuyện
            foreach (var item in history)
            {
                geminiRequest.Contents.Add(new GeminiContent
                {
                    Role = item.Role.ToLower(), // "user" hoặc "model"
                    Parts = new List<GeminiPart> { new GeminiPart { Text = item.Message } }
                });
            }

            // Khởi tạo tin nhắn mới
            geminiRequest.Contents.Add(new GeminiContent
            {
                Role = "user",
                Parts = new List<GeminiPart> { new GeminiPart { Text = userMessage } }
            });

            string fullUrl = $"{_geminiApiUrlBase}?key={_apiKey}";
            var response = await _httpClient.PostAsJsonAsync(fullUrl, geminiRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                // Log lỗi 
                throw new Exception($"Gemini API call failed: {response.StatusCode} - {errorBody}");
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<GeminiApiResponse>();

            // Trích xuất văn bản từ phản hồi
            var generatedText = apiResponse?.Candidates?.FirstOrDefault()
                                ?.Content?.Parts?.FirstOrDefault()?.Text;

            return generatedText ?? "Xin lỗi, tôi không thể tạo phản hồi vào lúc này.";
        }

        public async Task<string> GetWelcomeMessageAsync()
        {
            var systemInstruction = "Bạn là trợ lý ảo của công ty. Hãy tạo một câu chào mừng thân thiện, ngắn gọn (dưới 20 từ) để bắt đầu cuộc trò chuyện với khách hàng.";
            var userMessage = "Xin chào";

            return await GenerateChatResponseAsync(systemInstruction, new List<MessageHistoryItem>(), userMessage);
        }
    }
}