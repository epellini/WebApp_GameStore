using VirtualGameStore.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VirtualGameStore.Services
{
    public interface IGameStoreManager
    {
        // CRUD operations for Game entity:
        // Create Game:
        public void CreateGame(Game game);
        // Read all Games and sort:
        public List<Game> GetAllGames(string sort);
        // Read all Games by search query:
        public List<Game> GetGamesBySearch(string query);
        // Read Game:
        public Game? GetGameById(int id);
        // Update Game:
        public void UpdateGame(Game game);
        // Delete Game:
        public void DeleteGame(Game game);

        // CRUD operations for Profile entity:
        // Create Profile:
        public void CreateProfile(Profile profile);
        // Read Profile:
        public Profile GetProfileById(string id);
        // Update Profile:
        public void UpdateProfile(Profile profile);

        // Lists of misc user data for preferences/settings:
        // Read all preferred languages:
        public List<PreferredLanguage>? GetPreferredLanguagesById(string id);
        // Read all favourite genres:
        public List<FavouriteGenre>? GetFavouriteGenresById(string id);
        // Read all favourite platforms:
        public List<FavouritePlatform>? GetFavouritePlatformsById(string id);
        // Read all shipping addresses:
        public List<ShippingAddress>? GetShippingAddressesById(string id);
        // Read address:
        public ShippingAddress? GetAddressById(int id);


        // Read all platforms:
        public List<Platform> GetAllPlatforms();

        // CRUD operations for Picture entity:
        // Read Picture:
        public Picture GetPictureById(int id);

    }
}
