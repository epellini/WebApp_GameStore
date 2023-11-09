using Microsoft.EntityFrameworkCore;
using VirtualGameStore.DataAccess;
using VirtualGameStore.Entities;

namespace VirtualGameStore.Services
{
    // GameStoreManager implements an interface
    // Don't forget to add method signatures to interface too!
    public class GameStoreManager : IGameStoreManager
    {
        // Constructor to initialize DbContext field:
        public GameStoreManager(GameStoreDbContext gameStoreDbContext)
        {
            _gameStoreDbContext = gameStoreDbContext;
        }

        // Implement all interface methods:

        // CRUD operations for game entity:
        // Create Game:
        public void CreateGame(Game game)
        {
            _gameStoreDbContext.Games.Add(game);
            _gameStoreDbContext.SaveChanges();
        }

        // Read all Games and sort:
        /// <summary>
        /// Get all the games in the database including their platforms, genres, languages, and pictures.
        /// </summary>
        /// <returns>A list of Game objects</returns>
        public List<Game> GetAllGames(string sort)
        {
            List<Game> games = new List<Game>();
            if (sort == null) sort = "New";
            if (sort == "New")
            {
                games = _gameStoreDbContext.Games
                    .Include(g => g.Platforms).ThenInclude(p => p.Platform)
                    .Include(g => g.Genres).ThenInclude(ge => ge.Genre)
                    .Include(g => g.Languages).ThenInclude(l => l.Language)
                    .Include(g => g.Pictures)
                    .OrderByDescending(g => g.ReleaseDate)
                    .ToList();
            }
            if (sort == "Popular")
            {
                games = _gameStoreDbContext.Games
                .Include(g => g.Platforms).ThenInclude(p => p.Platform)
                .Include(g => g.Genres).ThenInclude(ge => ge.Genre)
                .Include(g => g.Languages).ThenInclude(l => l.Language)
                .Include(g => g.Pictures)
                .OrderBy(g => g.Genres.Count())
                .ToList();
            }
            if (sort == "Top")
            {
                games = _gameStoreDbContext.Games
                .Include(g => g.Platforms).ThenInclude(p => p.Platform)
                .Include(g => g.Genres).ThenInclude(ge => ge.Genre)
                .Include(g => g.Languages).ThenInclude(l => l.Language)
                .Include(g => g.Pictures)
                .ToList();
            }
            return games;
        }
        // Read all Games by search query:
        public List<Game> GetGamesBySearch(string query)
        {
            if (query != null)
            {
                return _gameStoreDbContext.Games
                .Include(g => g.Platforms).ThenInclude(p => p.Platform)
                .Include(g => g.Pictures)
                .Where(g => g.Name.Contains(query))
                .OrderByDescending(g => g.Name.StartsWith(query))
                .ThenBy(g => g.Name)
                .ToList();
            }
            else
            {
                return _gameStoreDbContext.Games
                .Include(g => g.Platforms).ThenInclude(p => p.Platform)
                .Include(g => g.Pictures)
                .OrderByDescending(g => g.ReleaseDate)
                .ToList();
            }
        }

        // Read Game:
        /// <summary>
        /// Get a single game from the database including its platforms, genres, languages, and pictures.
        /// </summary>
        /// <param name="id">The desired game's ID</param>
        /// <returns>A Game object</returns>
        public Game? GetGameById(int id)
        {
            return _gameStoreDbContext.Games
                .Include(g => g.Platforms).ThenInclude(p => p.Platform)
                .Include(g => g.Genres).ThenInclude(ge => ge.Genre)
                .Include(g => g.Languages).ThenInclude(l => l.Language)
                .Include(g => g.Pictures)
                .Where(g => g.GameId == id)
                .FirstOrDefault();
        }

        // Update Game:
        public void UpdateGame(Game game)
        {
            _gameStoreDbContext.Games.Update(game);
            _gameStoreDbContext.SaveChanges();
        }

        // Delete Game:
        public void DeleteGame(Game game)
        {
            _gameStoreDbContext.Remove(game);
            _gameStoreDbContext.SaveChanges();
        }

        // CRUD operations for Profile entity:
        // Create Profile:
        public void CreateProfile(Profile profile)
        {
            _gameStoreDbContext.Profiles.Add(profile);
            _gameStoreDbContext.SaveChanges();
        }
        // Read Profile:
        public Profile GetProfileById(string id)
        {
            return _gameStoreDbContext.Profiles
                .Include(p => p.Photos)
                .Where(p => p.UserId == id).FirstOrDefault();
        }
        // Update Profile:
        public void UpdateProfile(Profile profile)
        {
            if (GetProfileById(profile.UserId) == null)
            {
                CreateProfile(profile);
            }
            else
            {
                _gameStoreDbContext.Profiles.Update(profile);
                _gameStoreDbContext.SaveChanges();
            }
        }

        // CRUD operations for preferred languages entity:
        // Create preferred language:
        public void CreatePreferredLanguage(PreferredLanguage preferredLanguage)
        {
            _gameStoreDbContext.PreferredLanguages.Add(preferredLanguage);
            _gameStoreDbContext.SaveChanges();
        }
        // Read all preferred languages:
        public List<PreferredLanguage>? GetPreferredLanguagesById(string id)
        {
            return _gameStoreDbContext.PreferredLanguages.Include(pl => pl.Language).Where(pl => pl.UserId == id).ToList();
        }
        // Delete preferred language:
        public void DeletePreferredLanguage(PreferredLanguage preferredLanguage)
        {
            _gameStoreDbContext.PreferredLanguages.Remove(preferredLanguage);
            _gameStoreDbContext.SaveChanges();
        }

        // CRUD operations for favourite genres entity:
        // Create favourite genre:
        public void CreateFavouriteGenre(FavouriteGenre favouriteGenre)
        {
            _gameStoreDbContext.FavouriteGenres.Add(favouriteGenre);
            _gameStoreDbContext.SaveChanges();
        }
        // Read all favourite genres:
        public List<FavouriteGenre>? GetFavouriteGenresById(string id)
        {
            return _gameStoreDbContext.FavouriteGenres.Include(fg => fg.Genre).Where(fg => fg.UserId == id).ToList();
        }
        // Delete favourite genre:
        public void DeleteFavouriteGenre(FavouriteGenre favouriteGenre)
        {
            _gameStoreDbContext.FavouriteGenres.Remove(favouriteGenre);
            _gameStoreDbContext.SaveChanges();
        }

        // CRUD operations for favourite platforms entity:
        // Create favourite platform:
        public void CreateFavouritePlatform(FavouritePlatform favouritePlatform)
        {
            _gameStoreDbContext.FavouritePlatforms.Add(favouritePlatform);
            _gameStoreDbContext.SaveChanges();
        }
        // Read all favourite platforms:
        public List<FavouritePlatform>? GetFavouritePlatformsById(string id)
        {
            return _gameStoreDbContext.FavouritePlatforms.Include(fp => fp.Platform).Where(fp  => fp.UserId == id).ToList();
        }
        // Delete favourite platform:
        public void DeleteFavouritePlatform(FavouritePlatform favouritePlatform)
        {
            _gameStoreDbContext.FavouritePlatforms.Remove(favouritePlatform);
            _gameStoreDbContext.SaveChanges();
        }

        // CRUD operations for Shipping address entity:
        // Create Shipping address:
        public void CreateShippingAddress(ShippingAddress shippingAddress)
        {
            _gameStoreDbContext.ShippingAddresses.Add(shippingAddress);
            _gameStoreDbContext.SaveChanges();
        }
        // Read all Shipping addresses:
        public List<ShippingAddress>? GetShippingAddressesById(string id)
        {
            return _gameStoreDbContext.ShippingAddresses.Where(s => s.UserId == id).ToList();
        }
        // Read address:
        public ShippingAddress? GetAddressById(int id)
        {
            return _gameStoreDbContext.ShippingAddresses.Where(a => a.ShippingAddressId == id).FirstOrDefault();
        }
        // Update Shipping address:
        public void UpdateShippingAddress(ShippingAddress shippingAddress)
        {
            _gameStoreDbContext.ShippingAddresses.Update(shippingAddress);
            _gameStoreDbContext.SaveChanges();
        }
        // Delete Shipping address:
        public void DeleteShippingAddress(ShippingAddress shippingAddress)
        {
            _gameStoreDbContext.ShippingAddresses.Remove(shippingAddress);
            _gameStoreDbContext.SaveChanges();
        }

        // Read all Platforms:
        public List<Platform>? GetAllPlatforms()
        {
            return _gameStoreDbContext.Platforms.ToList();
        }
        // Read all genres:
        public List<Genre>? GetAllGenres()
        {
            return _gameStoreDbContext.Genres.ToList();
        }
        // Read all languages:
        public List<Language>? GetAllLanguages()
        {
            return _gameStoreDbContext.Languages.ToList();
        }

        // CRUD operations for Picture entity:
        // Read Picture:
        public Picture? GetPictureById(int id)
        {
            return _gameStoreDbContext.Pictures
                .Where(p => p.PictureId == id)
                .FirstOrDefault();
        }

        // CRUD operations for Photo entity:
        // Create Photo:
        public void CreatePhoto(IFormFile image, Photo photo)
        {
            photo.Image = ConvertImageToBytes(image);
            _gameStoreDbContext.Photos.Add(photo);
            _gameStoreDbContext.SaveChanges();
        }
        // Read Photo:
        public Photo? GetPhotoById(int photoId)
        {
            return _gameStoreDbContext.Photos
                .Where(p => p.PhotoId == photoId)
                .FirstOrDefault();
        }

        public byte[] ConvertImageToBytes(IFormFile image)
        {
            byte[] imageBytes;
            BinaryReader reader = new BinaryReader(image.OpenReadStream());
            imageBytes = reader.ReadBytes((int)image.Length);
            return imageBytes;
        }


        // private DbContext field
        private GameStoreDbContext _gameStoreDbContext;
    }
}
