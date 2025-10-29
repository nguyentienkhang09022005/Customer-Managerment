namespace Customer_Managerment.CustomerManagement.Domain.Entities
{
    public class PersonDomain
    {
        public Guid IdPerson { get; set; }

        public string? Fullname { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public decimal? Salary { get; set; }

        public string? Location { get; set; }

        public PersonDomain() { }
    }
}
