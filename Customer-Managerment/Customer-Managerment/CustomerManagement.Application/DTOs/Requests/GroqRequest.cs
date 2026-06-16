using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class GroqRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("messages")]
        public List<GroqMessage> Messages { get; set; }

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.3;
    }

    public class GroqMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
