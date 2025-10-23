namespace Customer_Managerment.CustomerManagement.Application.DTOs.Requests
{
    public class CampaignCreationRequest
    {
        public string? CampaignName { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public decimal? Budget { get; set; }

        public string? Description { get; set; }

        public Guid IdUser { get; set; }
    }
}
