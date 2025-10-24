using System;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class Lead
{
    public Guid IdLead { get; set; }

    public string? Resource { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual Person IdLeadNavigation { get; set; } = null!;
}
