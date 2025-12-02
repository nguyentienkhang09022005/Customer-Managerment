namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class QuantityStatisticsResponse
    {
        public decimal TotalProfit { get; set; }

        public int QuantityCustomers { get; set; }

        public int QuantityLeads { get; set; }

        public int QuantityContacts { get; set; }

        public QuantityStatisticsDetailContactResponse? quantityStatisticsDetailContactResponse { get; set; }

        public int QuantityDeals { get; set; }

        public QuantityStatisticsDetailDealResponse? quantityStatisticsDetailDealResponse { get; set; }
    }

    public class QuantityStatisticsDetailContactResponse
    {
        public int QuantityContactsPending { get; set; }

        public int QuantityContactsInProgress { get; set; }

        public int QuantityContactsDone { get; set; }

        public int QuantityContactsCancel { get; set; }

        public int QuantityContactsFailed { get; set; }
    }

    public class QuantityStatisticsDetailDealResponse
    {
        public int QuantityDealsPending { get; set; }

        public int QuantityDealsWon { get; set; }

        public int QuantityDealsLost { get; set; }
    }
}
