namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class ContactUpdateRequest
    {
        public string Type { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        public string? Status { get; set; }
    }
}
