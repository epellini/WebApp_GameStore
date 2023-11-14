using VirtualGameStore.Entities;

namespace VirtualGameStore.Models
{
    public class AllGamesViewModel
    {
        public List<Game>? Games { get; set; }
        public List<WishedGame>? WishedGames { get; set; }
    }
}
