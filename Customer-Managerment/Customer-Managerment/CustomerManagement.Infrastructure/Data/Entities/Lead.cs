using System;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class Lead
{
    public Guid IdLead { get; set; }

    public string? LeadName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? IdCampaign { get; set; }

    public virtual Campaign? IdCampaignNavigation { get; set; }
}
