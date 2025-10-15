namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class CompanyResponse
    {
        public Guid IdCompany { get; set; }

        public string? CompanyName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? TaxCode { get; set; }

        public string? Industry { get; set; }

        public DateTime? EstablishmentDate { get; set; }
    }
}
