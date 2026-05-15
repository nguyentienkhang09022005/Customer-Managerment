// // AI Gemini Request DTO - Commented out for future development
// using System.Collections.Generic;
// using System.Text.Json.Serialization;

// namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
// {
//     public class GeminiRequest
//     {
//         [JsonPropertyName("contents")]
//         public List<GeminiContent> Contents { get; set; }

//         [JsonPropertyName("systemInstruction")]
//         public SystemInstruction SystemInstruction { get; set; }
//     }

//     public class SystemInstruction
//     {
//         [JsonPropertyName("parts")]
//         public List<GeminiPart> Parts { get; set; }
//     }

//     public class GeminiContent
//     {
//         [JsonPropertyName("role")]
//         public string Role { get; set; }

//         [JsonPropertyName("parts")]
//         public List<GeminiPart> Parts { get; set; }
//     }

//     public class GeminiPart
//     {
//         [JsonPropertyName("text")]
//         public string Text { get; set; }
//     }
// }