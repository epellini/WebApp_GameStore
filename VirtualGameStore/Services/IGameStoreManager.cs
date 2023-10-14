using VirtualGameStore.Entities;

namespace VirtualGameStore.Services
{
    public interface IGameStoreManager
    {
        // CRUD operations for Game entity:
        // Create Game:
        public void CreateGame(Game game);
        // Read all Games:
        public List<Game> GetAllGames();
        // Read Game:
        public Game? GetGameById(int id);
        // Update Game:
        public void UpdateGame(Game game);
        // Delete Game:
        public void DeleteGame(Game game);


        public List<Platform> GetAllPlatforms();

        public Picture GetPictureById(int id);
    }
}
