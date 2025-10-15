namespace Customer_Managerment.CustomerManagement.Application.DTOs
{
    public class CompanyDTO
    {
        public Guid IdCompany { get; set; }

        public string? CompanyName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? TaxCode { get; set; }

        public string? Industry { get; set; }

        public DateOnly? EstablishmentDate { get; set; }
    }
}
