using System;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class Case
{
    public Guid IdCase { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ResolveAt { get; set; }

    public Guid? IdUser { get; set; }

    public Guid? IdCustomer { get; set; }

    public Guid? IdOrder { get; set; }

    public virtual Customer? IdCustomerNavigation { get; set; }

    public virtual Order? IdOrderNavigation { get; set; }

    public virtual User? IdUserNavigation { get; set; }
}
