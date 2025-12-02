namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class ChartDealResponse
    {
        public decimal SuccessfullDealValue { get; set; }

        public decimal FailedDealValue { get; set; }

        public List<ListSuccessfullDealResponse>? ListSuccessfullDeal { get; set; }

        public List<ListFailedDealResponse>? ListFailedDeal { get; set; }
    }

    public class ListSuccessfullDealResponse
    {
        public Guid IdDeal { get; set; }

        public decimal? Price { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }
    }

    public class ListFailedDealResponse
    {
        public Guid IdDeal { get; set; }

        public decimal? Price { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
