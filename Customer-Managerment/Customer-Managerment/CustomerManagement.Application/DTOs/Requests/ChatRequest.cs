using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class ChatRequest
    {
        public Guid IdStaff { get; set; }

        public string UserMessage { get; set; }
    }
}