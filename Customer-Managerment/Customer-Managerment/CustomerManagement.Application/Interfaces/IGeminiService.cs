using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Customer_Managerment.CustomerManagement.Application.Interfaces
{
    public interface IGeminiService
    {
        Task<string> GenerateChatResponseAsync(string systemInstruction,
                                               List<MessageHistoryItem> history,
                                               string userMessage);

        Task<string> GetWelcomeMessageAsync();
    }
}