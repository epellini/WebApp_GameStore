using VirtualGameStore.Entities;

namespace VirtualGameStore.Models
{
    public class ProfileViewModel
    {
        public User? User { get; set; }
        public Profile? Profile { get; set; }

        public bool IsOwner { get; set; }
        public bool IsSignedIn { get; set; }

        public List<Game>? Games { get; set; }
    }
}
