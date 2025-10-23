namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class LeadCreationRequest
    {
        public string? LeadName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public Guid? IdCampaign { get; set; }
    }
}
