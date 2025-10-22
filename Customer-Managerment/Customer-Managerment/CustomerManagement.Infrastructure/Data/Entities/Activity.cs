using System;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class Activity
{
    public Guid IdActivity { get; set; }

    public string? Type { get; set; }

    public string? Subject { get; set; }

    public string? Description { get; set; }

    public DateOnly? Date { get; set; }

    public Guid? IdUser { get; set; }

    public Guid? IdCustomer { get; set; }

    public virtual User? IdUserNavigation { get; set; }
}
