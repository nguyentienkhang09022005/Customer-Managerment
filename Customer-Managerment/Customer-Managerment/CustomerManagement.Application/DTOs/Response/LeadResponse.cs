using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class LeadResponse
    {
        public Guid IdLead { get; set; }

        public string? Resource { get; set; }

        public DateTime? CreatedAt { get; set; }

        public PersonResponse? personResponse { get; set; }
    }
}
