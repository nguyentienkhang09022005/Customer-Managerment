using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Services.GeminiModels
{
    public class GeminiApiResponse
    {
        [JsonPropertyName("candidates")]
        public List<GeminiCandidate> Candidates { get; set; }
    }

    public class GeminiCandidate
    {
        [JsonPropertyName("content")]
        public GeminiContent Content { get; set; }
    }
}