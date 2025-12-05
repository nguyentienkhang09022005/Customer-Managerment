namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class Contact
{
    public Guid IdContact { get; set; }

    public string Type { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid IdStaff { get; set; }

    public Guid IdLead { get; set; }

    public virtual Lead IdLeadNavigation { get; set; } = null!;

    public virtual Staff IdStaffNavigation { get; set; } = null!;
}
