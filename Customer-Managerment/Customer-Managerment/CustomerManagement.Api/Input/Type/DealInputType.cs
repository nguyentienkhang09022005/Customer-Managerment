using Customer_Managerment.CustomerManagement.Api.Input.Type.Enums;

namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class DealInput
    {
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public decimal Price { get; set; }
        public Guid IdStaff { get; set; }
        public Guid IdCustomer { get; set; }
    }

    public class DealUpdateInput
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public decimal? Price { get; set; }
        public DealStatus? Status { get; set; }
    }
}