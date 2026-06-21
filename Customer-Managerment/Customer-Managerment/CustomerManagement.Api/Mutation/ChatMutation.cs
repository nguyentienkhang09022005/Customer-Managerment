using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.UseCases;
using Microsoft.AspNetCore.Authorization;

namespace Customer_Managerment.CustomerManagement.Api.Mutation
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class ChatMutation
    {
        private readonly ChatHandler _chatHandler;

        public ChatMutation(ChatHandler chatHandler)
        {
            _chatHandler = chatHandler;
        }

        [Authorize]
        [GraphQLName("sendChatMessage")]
        public async Task<ChatResponse> SendChatMessageAsync(ChatRequest chatRequest)
        {
            return await _chatHandler.GenerateResponseAsync(chatRequest);
        }

        [Authorize]
        [GraphQLName("deleteMessage")]
        public async Task<string> DeleteMessageAsync(Guid idStaff)
        {
            return await _chatHandler.DeleteChatHistoryAsync(idStaff);
        }
    }
}
