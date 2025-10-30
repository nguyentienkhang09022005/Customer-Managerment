using Customer_Managerment.CustomerManagement.Application.DTOs.Requests;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
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