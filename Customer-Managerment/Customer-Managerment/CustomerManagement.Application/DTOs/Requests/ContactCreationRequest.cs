namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class ContactCreationRequest
    {
        public string? Type { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public Guid IdStaff { get; set; }

        public Guid IdLead { get; set; }
    }
}
