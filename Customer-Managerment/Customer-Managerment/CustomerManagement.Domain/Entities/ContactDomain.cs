namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class ContactDomain
    {
        public Guid IdContact { get; set; }

        public string Type { get; set; } = null!;

        public string Title { get; set; } = null!;  

        public string? Content { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public Guid IdStaff { get; set; }

        public Guid IdLead { get; set; }

        public ContactDomain(){}
    }
}
