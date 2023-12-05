using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using VirtualGameStore.Entities;

namespace VirtualGameStore.Models
{
    public class CartViewModel
    {
        // Properties:

        public int? ShoppingCartId { get; set; }
        public Cart? ShoppingCart { get; set; }
        public string? UserId { get; set; }
        public ICollection<Game>? shoppingCartGames { get; set; }

        public List<ShippingAddress>? ShippingAddresses { get; set; }

        [Required(ErrorMessage = "Please select a shipping address.")]
        public int? AddressId { get; set; }

        [Required(ErrorMessage = "Please enter your credit card number.")]
        // ensure it is 16 digits:
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Please enter a valid credit card number.")]
        public string? CreditCard { get; set; }

        [Required(ErrorMessage = "Please provide the expiry month")]
        // ensure it is 2 digits:
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Please enter a valid month.")]
        public string? ExpMonth { get; set; }

        [Required(ErrorMessage = "Please provide the expiry year")]
        // esnure it is 2 digits:
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Please enter a valid year.")]
        public string? ExpYear { get; set; }

        [Required(ErrorMessage = "Please enter the CVV")]
        // ensure it is 3 digits:
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Please enter a valid CVV.")]
        public string? CVV { get; set; }

        [Required(ErrorMessage = "Please enter your postal code.")]
        [Remote("CheckPostalCode", "Account")]
        public string? PostalCode { get; set; }
    }
}
