using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Microsoft.AspNetCore.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    [Authorize]
    public class ChatQuery
    {
        private readonly ChatHandler _chatHandler;

        public ChatQuery(ChatHandler chatHandler)
        {
            _chatHandler = chatHandler;
        }

        [GraphQLName("getChatWelcomeMessage")]
        public async Task<string> GetChatWelcomeMessageAsync()
        {
            return await _chatHandler.GetWelcomeMessageAsync();
        }

        [GraphQLName("getHistoryMessage")]
        public async Task<List<MessageHistoryItem>> GetHistoryMessageAsync(Guid idStaff)
        {
            return await _chatHandler.GetChatHistoryAsync(idStaff);
        }
    }
}
