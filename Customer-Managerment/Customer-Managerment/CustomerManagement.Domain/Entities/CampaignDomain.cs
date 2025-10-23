using Customer_Managerment.CustomerManagement.Domain.Exceptions;

namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class CampaignDomain
    {
        public Guid IdCampaign { get; set; }

        public string? CampaignName { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public decimal? Budget { get; set; }

        public string? Status { get; set; }

        public string? Description { get; set; }

        public Guid? IdUser { get; set; }

        CampaignDomain() { }
        public CampaignDomain(string? status)
        {
            CheckStatus(status);
            Status = status;
        }

        private void CheckStatus(string? status)
        {
            var allowed = new[] { "Active", "Inactive", "Completed" };

            if (string.IsNullOrWhiteSpace(status))
                throw new DomainException("Trạng thái chiến dịch không được để trống!", 400);

            if (!allowed.Contains(status, StringComparer.OrdinalIgnoreCase))
                throw new DomainException("Trạng thái chỉ được: Active, Inactive, Completed!", 400);

        }
    }
}
