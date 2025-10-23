using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using System.Text.Json;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class ChatHandler
    {
        private readonly IGeminiService _geminiService;
        private readonly ProductHandler _productHandler;
        private readonly IMapper _mapper;

        public ChatHandler(IGeminiService geminiService, ProductHandler productHandler, IMapper mapper)
        {
            _geminiService = geminiService;
            _productHandler = productHandler;
            _mapper = mapper;
        }

        public async Task<ChatResponse> GenerateResponseAsync(ChatRequest request)
        {
            // Lấy danh sách sản phẩm từ ProductHandler
            var products = await _productHandler.GetListProductsAsync();

            // Chuyển danh sách sản phẩm thành một chuỗi (JSON)
            var productInfo = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = false });

            // Tạo bối cảnh hệ thống với thông tin sản phẩm
            var systemInstruction = $"""
            Bạn là một trợ lý ảo thông minh và thân thiện của công ty.
            Nhiệm vụ của bạn là hỗ trợ khách hàng, trả lời các câu hỏi và tư vấn về sản phẩm.
            
            Đây là danh sách các sản phẩm của công ty (dưới dạng JSON):
            {productInfo}
            
            Hãy sử dụng thông tin này để trả lời các câu hỏi của khách hàng một cách chính xác.
            Luôn giữ thái độ chuyên nghiệp, lịch sự và hữu ích.
            """;

            var aiMessage = await _geminiService.GenerateChatResponseAsync(
                systemInstruction,
                request.History,
                request.UserMessage
            );

            var updatedHistory = new List<MessageHistoryItem>(request.History);
            updatedHistory.Add(new MessageHistoryItem { Role = "user", Message = request.UserMessage });
            updatedHistory.Add(new MessageHistoryItem { Role = "model", Message = aiMessage });

            return new ChatResponse
            {
                AiResponse = aiMessage,
                UpdatedHistory = updatedHistory
            };
        }

        public async Task<string> GetWelcomeMessageAsync()
        {
            return await _geminiService.GetWelcomeMessageAsync();
        }
    }
}