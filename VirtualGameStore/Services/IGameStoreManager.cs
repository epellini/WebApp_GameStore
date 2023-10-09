using VirtualGameStore.Entities;

namespace VirtualGameStore.Services
{
    public interface IGameStoreManager
    {
        public ICollection<Game> GetAllGames();
        
        public ICollection<Platform> GetAllPlatforms();

        public Picture GetPictureById(int id);
    }
}
