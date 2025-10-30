using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using HotChocolate.Authorization;
using System.Threading.Tasks;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class ChatQuery
    {
        private readonly ChatHandler _chatHandler;

        public ChatQuery(ChatHandler chatHandler)
        {
            _chatHandler = chatHandler;
        }

        //[Authorize]
        public async Task<string> GetChatWelcomeMessageAsync()
        {
            return await _chatHandler.GetWelcomeMessageAsync();
        }

        public async Task<List<MessageHistoryItem>> GetHistoryMessageAsync(Guid idStaff)
        {
            return await _chatHandler.GetChatHistoryAsync(idStaff);
        }
    }
}