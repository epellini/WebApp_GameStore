namespace VirtualGameStore.Entities
{
    public class FriendConnect
    {
        public int FriendConnectId { get; set; }

        // Foreign key properties:
        public string UserId { get; set; }
        public string FriendId { get; set; }

        // Navigation properties:
        public User? User { get; set; }
        public User? Friend { get; set; }

        // Properties
        public DateTime? DateConnected { get; set; }
        public string? Status { get; set; }
    }
}
