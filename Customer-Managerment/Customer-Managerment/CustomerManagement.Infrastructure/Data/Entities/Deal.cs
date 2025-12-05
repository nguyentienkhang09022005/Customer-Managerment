namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class Deal
{
    public Guid IdDeal { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public decimal? Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Status { get; set; }

    public Guid IdStaff { get; set; }

    public Guid IdCustomer { get; set; }

    public virtual Customer IdCustomerNavigation { get; set; } = null!;

    public virtual Staff IdStaffNavigation { get; set; } = null!;
}
