using Customer_Managerment.CustomerManagement.Application.DTOs.Response;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IGroqService
    {
        Task<string> GenerateChatResponseAsync(string systemInstruction,
                                               List<MessageHistoryItem> history,
                                               string userMessage);

        Task<string> GetWelcomeMessageAsync();
    }
}
