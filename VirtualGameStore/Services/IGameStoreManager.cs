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

        // LCRUD operations for preferredLanguage entity:
        // Create preferred language:
        public void CreatePreferredLanguage(PreferredLanguage preferredLanguage);
        // Read all preferred languages:
        public List<PreferredLanguage>? GetPreferredLanguagesById(string id);
        // Delete preferred language:
        public void DeletePreferredLanguage(PreferredLanguage preferredLanguage);

        // CRUD operations for FavouriteGenre entity:
        // Create favourite genre:
        public void CreateFavouriteGenre(FavouriteGenre favouriteGenre);
        // Read all favourite genres:
        public List<FavouriteGenre>? GetFavouriteGenresById(string id);
        // Delete favourite genre:
        public void DeleteFavouriteGenre(FavouriteGenre favouriteGenre);

        // LCRUD operations for WishedGame entity:
        // Create wished game:
        public void CreateWishedGame(WishedGame wishedGame);
        // Read all wished games:
        public List<WishedGame>? GetWishedGamesById(string id);
        // Delete wished game:
        public void DeleteWishedGame(WishedGame wishedGame);

        // CRUD operations for FavouritePlatform entity:
        // Create favourite platform:
        public void CreateFavouritePlatform(FavouritePlatform favouritePlatform);
        // Read all favourite platforms:
        public List<FavouritePlatform>? GetFavouritePlatformsById(string id);
        // Delete favourite platform:
        public void DeleteFavouritePlatform(FavouritePlatform favouritePlatform);

        // CRUD operations for ShippingAddress entity:
        // Create shipping address:
        public void CreateShippingAddress(ShippingAddress shippingAddress);
        // Read all shipping addresses:
        public List<ShippingAddress>? GetShippingAddressesById(string id);
        // Read address:
        public ShippingAddress? GetAddressById(int id);
        // Update shipping address:
        public void UpdateShippingAddress(ShippingAddress shippingAddress);
        // Delete shipping address:
        public void DeleteShippingAddress(ShippingAddress shippingAddress);


        // Read all platforms:
        public List<Platform>? GetAllPlatforms();
        // Read all genres:
        public List<Genre>? GetAllGenres();
        // Read all languages:
        public List<Language>? GetAllLanguages();

        // CRUD operations for Picture entity:
        // Read Picture:
        public Picture? GetPictureById(int id);

        // CRUD operations for Photo entity:
        // Create Photo:
        public void CreatePhoto(IFormFile image, Photo photo);
        // Read Photo:
        public Photo? GetPhotoById(int photoId);

        public byte[] ConvertImageToBytes(IFormFile image);

    }
}
