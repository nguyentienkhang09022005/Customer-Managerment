namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class Person
{
    public Guid IdPerson { get; set; }

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public decimal? Salary { get; set; }

    public string? Location { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Lead? Lead { get; set; }
}
