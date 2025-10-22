using System;
using System.Collections.Generic;

namespace Customer_Managerment.CustomerManagement.Application.DTOs.Response
{
    public class OrderResponse
    {
        public Guid IdOrder { get; set; }

        public string? Status { get; set; }

        public decimal? TotalAmount { get; set; }

        public string? PaymentMethod { get; set; }

        public DateTime? CreatedAt { get; set; }

        public List<OrderDetailResponse> Details { get; set; } = new();
    }

    public class OrderDetailResponse
    {
        public string? ProductName { get; set; }

        public int? Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? TotalPrice { get; set; }
    }
}
