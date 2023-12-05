using Microsoft.AspNetCore.Identity;

namespace VirtualGameStore.Entities
{
    public class User : IdentityUser
    {
        // Reference navigation properties to dependent entities:
        public ICollection<FavouritePlatform>? Platforms { get; set; }
        public ICollection<FavouriteGenre>? Genres { get; set; }
        public ICollection<PreferredLanguage>? Languages { get; set; }
        public ICollection<WishedGame>? WishedGames { get; set; }
        public ICollection<FriendConnect>? Friends { get; set; }
        public ICollection<FriendConnect>? Connects { get; set; }
        public Profile? Profile { get; set; }
        public ICollection<ShippingAddress>? ShippingAddresses { get; set; }
        public ICollection<EventRegistration>? EventRegistrations { get; set; }
        public ICollection<Rating>? Ratings { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public Cart? Cart { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
