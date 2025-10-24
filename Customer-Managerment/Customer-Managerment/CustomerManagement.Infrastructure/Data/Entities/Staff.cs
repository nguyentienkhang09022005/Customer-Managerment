using System;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class Staff
{
    public Guid IdStaff { get; set; }

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();
}
