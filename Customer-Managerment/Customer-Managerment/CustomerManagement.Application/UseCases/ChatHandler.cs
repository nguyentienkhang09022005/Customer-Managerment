using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Common.Exceptions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Customer_Managerment.CustomerManagement.Application.UseCases
{
    public class ChatHandler
    {
        private const int MaxHistoryTurns = 4;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = false,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            MaxDepth = 16
        };

        private const string FallbackSystemInstruction = """
            Bạn là trợ lý AI tên **CRMie** hỗ trợ nhân viên CRM Bất động sản.
            Quy tắc bảo mật: KHÔNG tiết lộ email, ID, password, token, hay dữ liệu nhạy cảm.
            Trả lời ngắn gọn, thân thiện, đúng trọng tâm. Không in JSON/SQL.
            Lưu ý: Dữ liệu tham khảo tạm thời không khả dụng do giới hạn kích thước request, hãy trả lời dựa trên ngữ cảnh cuộc trò chuyện và kiến thức chung.
            """;

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

            var listLeadJson = JsonSerializer.Serialize(listLead.Select(MapPersonToSummary), JsonOptions);
            var listCustomerJson = JsonSerializer.Serialize(listCustomer.Select(MapPersonToSummary), JsonOptions);
            var listDealJson = JsonSerializer.Serialize(listDeal.Select(MapDealToSummary), JsonOptions);
            var listContactJson = JsonSerializer.Serialize(listContact.Select(MapContactToSummary), JsonOptions);

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
            - Trả lời đúng trọng tâm câu hỏi, không lan man và không lặp lại cùng một nội dung.

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

            if (history.Count > MaxHistoryTurns)
            {
                history = history.Skip(history.Count - MaxHistoryTurns).ToList();
            }

            string aiMessage;
            try
            {
                aiMessage = await _groqService.GenerateChatResponseAsync(
                    systemInstruction,
                    history,
                    request.UserMessage
                );
            }
            catch (GroqRequestTooLargeException ex)
            {
                _logger.LogWarning(ex, "Groq request too large on first attempt, retrying with fallback context (history turns: {Turns}).", history.Count);

                var fallbackHistory = history.TakeLast(2).ToList();
                aiMessage = await _groqService.GenerateChatResponseAsync(
                    FallbackSystemInstruction,
                    fallbackHistory,
                    request.UserMessage
                );
            }

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

        private static PersonSummary MapPersonToSummary(Person p) => new(
            Id: ShortId(p.Id),
            Name: p.Fullname,
            Role: p.Role,
            Status: p.Status switch
            {
                1 => "active",
                0 => "inactive",
                _ => p.Status.ToString()
            },
            Location: p.Location,
            Phone: MaskPhone(p.Phone),
            Resource: p.Resource
        );

        private static DealSummary MapDealToSummary(Deal d) => new(
            Id: ShortId(d.IdDeal),
            Title: d.Title,
            Status: d.Status,
            Price: d.Price,
            CustomerName: d.IdCustomerNavigation?.Fullname,
            StaffName: d.IdStaffNavigation?.Fullname
        );

        private static ContactSummary MapContactToSummary(Contact c) => new(
            Id: ShortId(c.IdContact),
            Title: c.Title,
            Status: c.Status,
            Type: c.Type,
            LeadName: c.IdLeadNavigation?.Fullname,
            StaffName: c.IdStaffNavigation?.Fullname
        );

        private static string ShortId(Guid id) => id.ToString("N")[..8];

        private static string? MaskPhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone) || phone.Length < 6) return phone;
            return $"{phone[..3]}***{phone[^3..]}";
        }

        private record PersonSummary(string Id, string Name, string? Role, string? Status, string? Location, string? Phone, string? Resource);
        private record DealSummary(string Id, string Title, string? Status, decimal Price, string? CustomerName, string? StaffName);
        private record ContactSummary(string Id, string Title, string? Status, string Type, string? LeadName, string? StaffName);
    }
}
