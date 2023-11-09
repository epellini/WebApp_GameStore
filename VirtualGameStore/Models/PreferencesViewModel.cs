using VirtualGameStore.Entities;

namespace VirtualGameStore.Models
{
    public class PreferencesViewModel
    {
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public List<Platform>? AllPlatforms { get; set; }
        public List<Genre>? AllGenres { get; set; }
        public List<Language>? AllLanguages { get; set; }

        public List<FavouritePlatform>? FavPlatforms { get; set; }
        public List<FavouriteGenre>? FavGenres { get; set; }
        public List<PreferredLanguage>? PrefLanguages { get; set; }

        public string? Platforms { get; set; }
        public string? Genres { get; set; }
        public string? Languages { get; set; }
    }
}
