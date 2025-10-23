using System;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class Campaign
{
    public Guid IdCampaign { get; set; }

    public string? CampaignName { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal? Budget { get; set; }

    public string? Status { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? IdUser { get; set; }

    public virtual User? IdUserNavigation { get; set; }

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}
