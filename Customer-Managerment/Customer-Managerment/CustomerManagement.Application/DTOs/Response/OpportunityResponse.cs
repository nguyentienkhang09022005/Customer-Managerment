namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class OpportunityResponse
    {
        public Guid IdOpportunity { get; set; }

        public string? OpportunityName { get; set; }

        public string? Stage { get; set; }

        public decimal? Amount { get; set; }

        public DateOnly? ExpectedClosedDate { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
