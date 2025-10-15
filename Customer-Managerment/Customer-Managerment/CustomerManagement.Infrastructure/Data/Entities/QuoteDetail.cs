using System;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class QuoteDetail
{
    public Guid IdQuoteDetail { get; set; }

    public int? Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    public Guid? IdQuote { get; set; }

    public Guid? IdProduct { get; set; }

    public virtual Product? IdProductNavigation { get; set; }

    public virtual Quote? IdQuoteNavigation { get; set; }
}
