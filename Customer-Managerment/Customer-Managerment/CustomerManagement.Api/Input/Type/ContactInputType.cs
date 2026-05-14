using Customer_Managerment.CustomerManagement.Api.Input.Type.Enums;

namespace Customer_Managerment.CustomerManagement.Api.Input.Type
{
    public class ContactInput
    {
        public string Type { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public Guid IdStaff { get; set; }
        public Guid IdLead { get; set; }
    }

    public class ContactUpdateInput
    {
        public string? Type { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public ContactStatus? Status { get; set; }
    }
}