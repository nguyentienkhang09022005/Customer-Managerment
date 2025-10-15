using System;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class Opportunity
{
    public Guid IdOpportunity { get; set; }

    public string? OpportunityName { get; set; }

    public string? Stage { get; set; }

    public decimal? Amount { get; set; }

    public DateOnly? ExpectedClosedDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? IdUser { get; set; }

    public virtual User? IdUserNavigation { get; set; }

    public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();
}
