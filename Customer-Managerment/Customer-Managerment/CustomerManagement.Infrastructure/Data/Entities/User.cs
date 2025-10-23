using System;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class User
{
    public Guid IdUser { get; set; }

    public string? Fullname { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    // Người tạo
    public ICollection<Activity> CreatedActivities { get; set; } = new List<Activity>();

    // Là khách hàng
    public ICollection<Activity> CustomerActivities { get; set; } = new List<Activity>();

    public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();

    public virtual ICollection<Case> Cases { get; set; } = new List<Case>();

    public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
}
