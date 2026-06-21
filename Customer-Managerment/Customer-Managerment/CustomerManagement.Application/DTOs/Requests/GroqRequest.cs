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
        public double Temperature { get; set; } = 0.15;

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; } = 600;

        [JsonPropertyName("top_p")]
        public double TopP { get; set; } = 0.9;

        [JsonPropertyName("frequency_penalty")]
        public double FrequencyPenalty { get; set; } = 0.3;

        [JsonPropertyName("presence_penalty")]
        public double PresencePenalty { get; set; } = 0.1;

        [JsonPropertyName("stop")]
        public List<string>? Stop { get; set; } = new List<string> { "\n\n\n", "assistant\n", "user\n" };
    }

    public class GroqMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
