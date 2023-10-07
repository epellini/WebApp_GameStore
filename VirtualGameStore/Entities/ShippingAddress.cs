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
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? DeliveryInstructions { get; set; }

    }
}
