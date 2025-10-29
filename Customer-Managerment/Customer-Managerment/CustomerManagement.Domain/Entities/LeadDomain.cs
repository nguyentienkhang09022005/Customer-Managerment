using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class LeadDomain
    {
        public Guid IdLead { get; set; }

        public string? Resource { get; set; }

        public DateTime? CreatedAt { get; set; }

        public PersonDomain personDomain { get; set; }

        public virtual Person IdLeadNavigation { get; set; } = null!;
    }
}
