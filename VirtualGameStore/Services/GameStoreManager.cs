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

        // Read all Games:
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

        public Profile GetProfileById(string id)
        {
            return _gameStoreDbContext.Profiles.Where(p => p.UserId == id).FirstOrDefault();
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

        public List<Platform> GetAllPlatforms()
        {
            throw new NotImplementedException();
        }

        public Picture GetPictureById(int id)
        {
            return _gameStoreDbContext.Pictures
                .Where(p => p.PictureId == id)
                .FirstOrDefault();
        }


        // private DbContext field
        private GameStoreDbContext _gameStoreDbContext;
    }
}
