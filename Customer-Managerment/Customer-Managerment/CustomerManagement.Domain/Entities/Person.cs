namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public enum PersonType
    {
        Staff,
        Lead,
        Customer
    }

    public class Person : BaseEntity
    {
        public string Fullname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public PersonType Discriminator { get; set; }

        // Staff-specific properties
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? Role { get; set; }
        public decimal? Salary { get; set; }
        public int Status { get; set; } = 0;
        public DateTime? LastActiveAt { get; set; }

        // Lead-specific properties
        public string? Resource { get; set; }

        // Navigation properties for derived classes
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();
    }
}