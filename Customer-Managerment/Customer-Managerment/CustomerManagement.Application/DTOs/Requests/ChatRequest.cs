using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class ChatRequest
    {
        public string UserMessage { get; set; }
        public List<MessageHistoryItem> History { get; set; } = new List<MessageHistoryItem>();
    }
}