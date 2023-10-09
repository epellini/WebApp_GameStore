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
        public List<Game> GetAllGames()
        {
            return _gameStoreDbContext.Games
                .Include(g => g.Platforms).ThenInclude(p => p.Platform)
                .Include(g => g.Genres).ThenInclude(ge => ge.Genre)
                .Include(g => g.Languages).ThenInclude(l => l.Language)
                .Include(g => g.Pictures)
                .ToList();
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
