using System;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;

public partial class Tasks
{
    public Guid IdTask { get; set; }

    public string? Title { get; set; }

    public DateOnly? DueDate { get; set; }

    public string? Status { get; set; }

    public Guid? IdUser { get; set; }

    public virtual User? IdUserNavigation { get; set; }
}
