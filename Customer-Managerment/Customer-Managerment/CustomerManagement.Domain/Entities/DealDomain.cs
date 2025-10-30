namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class DealDomain
    {
        public Guid IdDeal { get; set; }

        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        public decimal? Price { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? Status { get; set; }

        public Guid IdStaff { get; set; }

        public Guid IdCustomer { get; set; }

        public CustomerDomain? Customer { get; set; }

        public StaffDomain? Staff { get; set; }

        public DealDomain() { }
    }
}
