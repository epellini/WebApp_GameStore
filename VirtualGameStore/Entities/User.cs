using Microsoft.AspNetCore.Identity;

namespace VirtualGameStore.Entities
{
    public class User : IdentityUser
    {
        // Reference navigation properties to dependent entities:
        public ICollection<FavouritePlatform>? Platforms { get; set; }
        public ICollection<FavouriteGenre>? Genres { get; set; }
        public ICollection<PreferredLanguage>? Languages { get; set; }
        public ICollection<WishedGames>? WishedGames { get; set; }
        public Profile? Profile { get; set; }
        public ICollection<ShippingAddress>? ShippingAddresses { get; set; }
    }
}
