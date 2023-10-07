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

        public ICollection<Game> GetAllGames()
        {
            return _gameStoreDbContext.Games.Include(g => g.Platforms).Include(g => g.Genres).Include(g => g.Languages).ToList();
        }

        public ICollection<Platform> GetAllPlatforms()
        {
            throw new NotImplementedException();
        }


        // private DbContext field
        private GameStoreDbContext _gameStoreDbContext;
    }
}
