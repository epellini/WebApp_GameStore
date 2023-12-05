using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace VirtualGameStore.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public int ShippingAddressId { get; set; }
        public User? User { get; set; }
        public ShippingAddress? ShippingAddress { get; set; }
        public ICollection<OrderItem>? Items { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }

        public string? Status { get; set; } // Paid, Shipped, Completed, Cancelled
        public DateTime OrderDate { get; set; }
        // Properties:
        [Required(ErrorMessage = "Please enter your full name as it appears on your card.")]
        public string? BillingName { get; set; }

        public string? CardNumber { get; set; }
        public string? SecurityCode { get; set; }
        public int? ExpirationMonth { get; set; }
        public int? ExpirationYear { get; set; }

        public string? BillingAddress { get; set; }

        public string? BillingAddress2 { get; set; }


        public string? BillingCity { get; set; }

  
        public string? BillingProvince { get; set; }


        public string? BillingPostalCode { get; set; }


        public string? BillingCountry { get; set; }
    }
}
