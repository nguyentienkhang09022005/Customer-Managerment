namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class LeadDomain
    {
        public Guid IdLead { get; set; }

        public string? LeadName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public DateTime? CreatedAt { get; set; }

        public Guid? IdCampaign { get; set; }

        public LeadDomain(){}     
    }
}
