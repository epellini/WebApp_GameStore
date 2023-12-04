using VirtualGameStore.Entities;

namespace VirtualGameStore.Models
{
    public class AdminPanelViewModel
    {
        public List<Game>? AllGames { get; set; }
        public List<Event>? AllEvents { get; set; }
    }
}
