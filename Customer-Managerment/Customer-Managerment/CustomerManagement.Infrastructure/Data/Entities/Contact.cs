using System;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class Contact
{
    public Guid IdContact { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Position { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? IdCustomer { get; set; }

    public virtual Customer? IdCustomerNavigation { get; set; }
}
