namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class GroqSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.groq.com/openai/v1";
        public string Model { get; set; } = "meta-llama/llama-4-scout-17b-16e-instruct";
        public double Temperature { get; set; } = 0.3;
        public int TimeoutSeconds { get; set; } = 120;
    }
}
