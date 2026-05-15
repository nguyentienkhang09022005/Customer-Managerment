// // AI Chat Mutation - Commented out for future development
// using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
// using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
// using Customer_Managerment.CustomerManagement.Application.UseCases;

// namespace Customer_Managerment.CustomerManagement.Api.Mutation
// {
//     [ExtendObjectType(OperationTypeNames.Mutation)]
//     public class ChatMutation
//     {
//         private readonly ChatHandler _chatHandler;

//         public ChatMutation(ChatHandler chatHandler)
//         {
//             _chatHandler = chatHandler;
//         }

//         //[Authorize]
//         public async Task<ChatResponse> SendChatMessageAsync(ChatRequest chatRequest)
//         {
//             return await _chatHandler.GenerateResponseAsync(chatRequest);
//         }

//         public async Task<string> DeleteMessageAsync(Guid idStaff)
//         {
//             return await _chatHandler.DeleteChatHistoryAsync(idStaff);
//         }
//     }
// }