using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class GroqApiResponse
    {
        [JsonPropertyName("choices")]
        public List<GroqChoice> Choices { get; set; }
    }

    public class GroqChoice
    {
        [JsonPropertyName("message")]
        public GroqResponseMessage Message { get; set; }
    }

    public class GroqResponseMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
