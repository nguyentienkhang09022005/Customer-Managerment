using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using System.Text.Json;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class ChatHandler
    {
        private readonly IGroqService _groqService;
        private readonly IChatHistoryService _chatHistoryService;
        private readonly IMapper _mapper;
        private readonly ILeadRepository _leadRepository;
        private readonly IContactRepository _contactRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IDealRepository _dealRepository;
        private readonly ILogger<ChatHandler> _logger;


        public ChatHandler(IGroqService groqService,
                           IChatHistoryService chatHistoryService,
                           IMapper mapper,
                           ILeadRepository leadRepository,
                           IContactRepository contactRepository,
                           ICustomerRepository customerRepository,
                           IDealRepository dealRepository,
                           ILogger<ChatHandler> logger)
        {
            _groqService = groqService;
            _chatHistoryService = chatHistoryService;
            _mapper = mapper;
            _leadRepository = leadRepository;
            _contactRepository = contactRepository;
            _customerRepository = customerRepository;
            _dealRepository = dealRepository;
            _logger = logger;
        }

        public async Task<ChatResponse> GenerateResponseAsync(ChatRequest request)
        {
            var listLead = await _leadRepository.GetListLeadAsync();
            var listCustomer = await _customerRepository.GetListCustomerAsync();
            var listDeal = await _dealRepository.GetListDealAsync();
            var listContact = await _contactRepository.GetListContactAsync();

            var jsonOptions = new JsonSerializerOptions { WriteIndented = false };

            var listLeadJson = JsonSerializer.Serialize(listLead, jsonOptions);
            var listCustomerJson = JsonSerializer.Serialize(listCustomer, jsonOptions);
            var listDealJson = JsonSerializer.Serialize(listDeal, jsonOptions);
            var listContactJson = JsonSerializer.Serialize(listContact, jsonOptions);

            var systemInstruction = $"""
            Bạn là một trợ lý AI chuyên nghiệp tên gọi là **CRMie**, được tích hợp trong hệ thống CRM Bất động sản của công ty.
            Nhiệm vụ của bạn là hỗ trợ Nhân viên (Staff) trong các nghiệp vụ quản lý Leads, Customers, Contacts và Deals.

            **Quy tắc bảo mật nghiêm ngặt:**
            - Tuyệt đối không tiết lộ ID, email, token, password, hoặc bất kỳ thông tin kỹ thuật nhạy cảm nào.
            - Không trả trực tiếp toàn bộ JSON hay dữ liệu thô từ database.
            - Không thực hiện truy vấn SQL hoặc bất kỳ hành động nào có thể gây rủi ro SQL injection.
            - Chỉ cung cấp thông tin ở mức tổng quan, số liệu đã được làm ẩn thông tin nhạy cảm.

            **Cách trả lời:**
            - Giải thích rõ ràng, ngắn gọn, thân thiện và chuyên nghiệp.
            - Thêm emoji phù hợp nếu cần để giao tiếp tự nhiên.
            - Nếu dữ liệu bị thiếu, trả lời nhẹ nhàng, không đoán dữ liệu.
            - Trả lời chi ý rõ ràng, không lan man và không trả lời trên 1 dòng.

            **Ví dụ câu hỏi hợp lệ:**
            - "Tóm tắt thông tin của Lead 'Nguyễn Văn A'?"
            - "Tôi có bao nhiêu deal đang ở trạng thái 'Pending'?"
            - "Gợi ý nội dung email chăm sóc Customer 'Trần Thị B'?"

            **Dữ liệu tham khảo (đã được ẩn thông tin nhạy cảm):**
            - Danh sách Lead: {listLeadJson}
            - Danh sách Customer: {listCustomerJson}
            - Danh sách Deal: {listDealJson}
            - Danh sách Contact: {listContactJson}

            **Hướng dẫn AI:**
            - Luôn trả lời theo vai trò là trợ lý của nhân viên, không trả lời như khách hàng bên ngoài.
            - Không tiết lộ bất kỳ thông tin nhạy cảm nào.
            - Không in trực tiếp JSON hay SQL query.
            - Chỉ cung cấp thông tin cần thiết để hỗ trợ nghiệp vụ của nhân viên.
            """;

            var history = await _chatHistoryService.GetHistoryAsync(request.IdStaff);

            var aiMessage = await _groqService.GenerateChatResponseAsync(
                systemInstruction,
                history,
                request.UserMessage
            );
            aiMessage = aiMessage.Replace("\\n", "\n").Replace("\\r", "");

            await _chatHistoryService.SaveMessageAsync(request.IdStaff, new MessageHistoryItem { Role = "user", Message = request.UserMessage });
            await _chatHistoryService.SaveMessageAsync(request.IdStaff, new MessageHistoryItem { Role = "model", Message = aiMessage });

            return new ChatResponse
            {
                AiResponse = aiMessage
            };
        }

        public async Task<string> GetWelcomeMessageAsync()
        {
            return await _groqService.GetWelcomeMessageAsync();
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
