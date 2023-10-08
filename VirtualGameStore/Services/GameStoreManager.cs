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

        /// <summary>
        /// Get all the games in the database including their platforms, genres, and languages.
        /// </summary>
        /// <returns>A list of Game objects</returns>
        public ICollection<Game> GetAllGames()
        {
            return _gameStoreDbContext.Games
                .Include(g => g.Platforms).ThenInclude(p => p.Platform)
                .Include(g => g.Genres).ThenInclude(ge => ge.Genre)
                .Include(g => g.Languages).ThenInclude(l => l.Language)
                .ToList();
        }

        public ICollection<Platform> GetAllPlatforms()
        {
            throw new NotImplementedException();
        }


        // private DbContext field
        private GameStoreDbContext _gameStoreDbContext;
    }
}
