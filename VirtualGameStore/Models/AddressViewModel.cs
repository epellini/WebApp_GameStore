using VirtualGameStore.Entities;

namespace VirtualGameStore.Models
{
    public class AddressViewModel
    {
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public List<ShippingAddress>? ShippingAddresses { get; set; }
    }
}
