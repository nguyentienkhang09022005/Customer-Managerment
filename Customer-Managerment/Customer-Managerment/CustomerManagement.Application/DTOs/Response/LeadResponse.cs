namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class LeadResponse
    {
        public Guid IdLead { get; set; }

        public string? LeadName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public DateTime? CreatedAt { get; set; }

        public Guid? IdCampaign { get; set; }
    }
}
