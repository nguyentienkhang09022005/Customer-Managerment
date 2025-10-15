using System;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class Order
{
    public Guid IdOrder { get; set; }

    public DateOnly? OrderDate { get; set; }

    public string? Status { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? PaymentMethod { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? IdUser { get; set; }

    public Guid? IdCustomer { get; set; }

    public virtual ICollection<Case> Cases { get; set; } = new List<Case>();

    public virtual Customer? IdCustomerNavigation { get; set; }

    public virtual User? IdUserNavigation { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
