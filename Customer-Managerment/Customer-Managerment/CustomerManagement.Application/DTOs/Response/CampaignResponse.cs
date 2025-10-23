namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class CampaignResponse
    {
        public Guid IdCampaign { get; set; }

        public string? CampaignName { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public decimal? Budget { get; set; }

        public string? Status { get; set; }

        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
