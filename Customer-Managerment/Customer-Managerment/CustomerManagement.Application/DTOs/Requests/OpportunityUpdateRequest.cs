namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class OpportunityUpdateRequest
    {
        public string? OpportunityName { get; set; }

        public string? Stage { get; set; }

        public decimal? Amount { get; set; }

        public DateOnly? ExpectedClosedDate { get; set; }
    }
}
