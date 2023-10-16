using VirtualGameStore.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VirtualGameStore.Services
{
    public interface IGameStoreManager
    {
        // CRUD operations for Game entity:
        // Create Game:
        public void CreateGame(Game game);
        // Read all Games:
        public List<Game> GetAllGames(string sort);
        public List<Game> GetGamesBySearch(string query);
        // Read Game:
        public Game? GetGameById(int id);
        // Update Game:
        public void UpdateGame(Game game);
        // Delete Game:
        public void DeleteGame(Game game);

        public Profile GetProfileById(string id);
        public List<PreferredLanguage> GetPreferredLanguagesById(string id);
        public List<FavouriteGenre> GetFavouriteGenreById(string id);
        public List<FavouritePlatform> GetFavouritePlatformById(string id);
        public List<ShippingAddress> GetShippingAddressesById(string id);
        public List<Platform> GetAllPlatforms();

        public Picture GetPictureById(int id);
    }
}
