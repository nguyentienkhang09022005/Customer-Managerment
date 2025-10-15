using System;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class Customer
{
    public Guid IdCustomer { get; set; }

    public string? CustomerName { get; set; }

    public string? Industry { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? AccountType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? IdCampaign { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public virtual ICollection<Case> Cases { get; set; } = new List<Case>();

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual Campaign? IdCampaignNavigation { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
