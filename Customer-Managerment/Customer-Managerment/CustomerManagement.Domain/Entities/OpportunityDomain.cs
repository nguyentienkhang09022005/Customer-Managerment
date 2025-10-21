namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class OpportunityDomain
    {
        public Guid IdOpportunity { get; set; }

        public string? OpportunityName { get; set; }

        public string? Stage { get; set; }

        public decimal? Amount { get; set; }

        public DateOnly? ExpectedClosedDate { get; set; }

        public DateTime? CreatedAt { get; set; }

        public Guid? IdUser { get; set; }
    }
}
