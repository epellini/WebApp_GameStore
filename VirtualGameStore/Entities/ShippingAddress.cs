using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace VirtualGameStore.Entities
{
    public class ShippingAddress
    {
        // Primary key property:
        public int ShippingAddressId { get; set; }

        // Foreign key properties:
        public string? UserId { get; set; }

        // Reference navigation property to principal entity for each foreign key:
        public User? User { get; set; }

        // Properties:
        [Required(ErrorMessage = "Please enter your full name.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Please enter your phone number.")]
        [Remote("CheckPhoneNumber", "Account")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Please enter your address.")]
        [Remote("CheckAddress", "Account")]
        public string? Address { get; set; }

        public string? Address2 { get; set; }

        [Required(ErrorMessage = "Please enter your city.")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Please select your province.")]
        public string? Province { get; set; }

        [Required(ErrorMessage = "Please enter your postal code.")]
        [Remote("CheckPostalCode", "Account")]
        public string? PostalCode { get; set; }

        [Required(ErrorMessage = "Please select your country.")]
        public string? Country { get; set; } = "Canada";

        public string? DeliveryInstructions { get; set; }

        public bool IsDefault { get; set; } = false;

        public ICollection<Order>? Orders { get; set; }

    }
}
