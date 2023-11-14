using VirtualGameStore.Entities;

namespace VirtualGameStore.Models
{
    public class ProfileViewModel
    {
        public User? User { get; set; }
        public Profile? Profile { get; set; }

        public bool IsOwner { get; set; }
        public bool IsSignedIn { get; set; }

        public List<Game>? Library { get; set; }

        public List<WishedGame>? WishedGames { get; set; }
        public List<FriendConnect>? Friends { get; set; }
        public FriendConnect? ExistingFriend { get; set; }
    }
}
