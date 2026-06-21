namespace Customer_Managerment.CustomerManagement.Application.Common.Exceptions
{
    public class GroqRequestTooLargeException : Exception
    {
        public int StatusCode { get; }
        public string ResponseBody { get; }

        public GroqRequestTooLargeException(int statusCode, string responseBody)
            : base($"Groq API request too large (HTTP {statusCode}). Response: {responseBody}")
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }
    }
}
