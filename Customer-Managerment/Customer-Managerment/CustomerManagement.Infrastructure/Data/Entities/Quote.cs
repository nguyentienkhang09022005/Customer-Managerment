using System;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class Quote
{
    public Guid IdQuote { get; set; }

    public DateOnly? QuoteDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Status { get; set; }

    public DateOnly? ExpiryTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? IdOpportunity { get; set; }

    public virtual Opportunity? IdOpportunityNavigation { get; set; }

    public virtual ICollection<QuoteDetail> QuoteDetails { get; set; } = new List<QuoteDetail>();
}
