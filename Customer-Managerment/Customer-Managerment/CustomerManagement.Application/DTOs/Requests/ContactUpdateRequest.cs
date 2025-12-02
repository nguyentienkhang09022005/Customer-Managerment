namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class ContactUpdateRequest
    {
        public string? Type { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? Status { get; set; }
    }
}
