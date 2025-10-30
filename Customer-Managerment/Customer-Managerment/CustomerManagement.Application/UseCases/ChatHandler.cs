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
        private readonly IChatHistoryService _chatHistoryService;
        private readonly IMapper _mapper;

        public ChatHandler(IGeminiService geminiService, IChatHistoryService chatHistoryService, IMapper mapper)
        {
            _geminiService = geminiService;
            _chatHistoryService = chatHistoryService;
            _mapper = mapper;
        }

        public async Task<ChatResponse> GenerateResponseAsync(ChatRequest request)
        {
            // Tạo bối cảnh hệ thống
            var systemInstruction = $"""
            Bạn là một trợ lý AI chuyên nghiệp, được tích hợp trong hệ thống CRM Bất động sản của công ty.
            Nhiệm vụ của bạn là hỗ trợ Nhân viên (Staff) trong các nghiệp vụ hàng ngày.
            Người dùng đang chat với bạn chính là một nhân viên (Staff) của công ty.

            Hãy trả lời các câu hỏi của nhân viên liên quan đến việc quản lý Khách hàng tiềm năng (Leads),
            Khách hàng (Customers), Tương tác (Contacts), và Giao dịch (Deals).

            Ví dụ, nhân viên có thể hỏi:
            - "Tóm tắt thông tin của Lead 'Nguyễn Văn A'?"
            - "Tôi có bao nhiêu deal đang ở trạng thái 'Pending'?"
            - "Gợi ý nội dung email để chăm sóc Customer 'Trần Thị B'?"

            Hãy luôn giữ thái độ chuyên nghiệp, chính xác và hỗ trợ.
            Bạn đang nói chuyện với đồng nghiệp (nhân viên), không phải khách hàng bên ngoài.
            """;

            // Lấy lịch sử trò chuyện và gửi yêu cầu đến Gemini AI
            var history = await _chatHistoryService.GetHistoryAsync(request.IdStaff);
            var aiMessage = await _geminiService.GenerateChatResponseAsync(
                systemInstruction,
                history,
                request.UserMessage
            );
            aiMessage = aiMessage.Replace("\\n", "\n").Replace("\\r", "");

            // Lưu vào Redis
            await _chatHistoryService.SaveMessageAsync(request.IdStaff, new MessageHistoryItem { Role = "staff", Message = request.UserMessage });
            await _chatHistoryService.SaveMessageAsync(request.IdStaff, new MessageHistoryItem { Role = "model", Message = aiMessage });

            return new ChatResponse
            {
                AiResponse = aiMessage
            };
        }

        public async Task<string> GetWelcomeMessageAsync()
        {
            return await _geminiService.GetWelcomeMessageAsync();
        }

        public async Task<List<MessageHistoryItem>> GetChatHistoryAsync(Guid idStaff)
        {
            var history = await _chatHistoryService.GetHistoryAsync(idStaff);
            return history;
        }

        public async Task<string> DeleteChatHistoryAsync(Guid idStaff)
        {
            await _chatHistoryService.DeleteHistoryAsync(idStaff);
            return "Xóa lịch sử trò chuyện thành công!";
        }
    }
}