using System;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class Customer
{
    public Guid IdCustomer { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();

    public virtual Person IdCustomerNavigation { get; set; } = null!;
}
