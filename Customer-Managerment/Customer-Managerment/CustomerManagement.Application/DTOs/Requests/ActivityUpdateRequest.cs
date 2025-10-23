namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class ActivityUpdateRequest
    {
        public string? Type { get; set; }

        public string? Subject { get; set; }

        public string? Description { get; set; }

        public DateOnly? Date { get; set; }
    }
}
