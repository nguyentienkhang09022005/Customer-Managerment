namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class ActivityCreationRequest
    {
        public string? Type { get; set; }

        public string? Subject { get; set; }

        public string? Description { get; set; }

        public DateOnly? Date { get; set; }

        public Guid IdUser { get; set; }

        public Guid IdCustomer { get; set; }
    }
}
