namespace VirtualGameStore.Entities
{
    public class WishedGame
    {
        // Primary key properties:
        public int WishedGameId { get; set; }

        // Foreign key properties:
        public string UserId { get; set; }
        public int GameId { get; set; }

        // Navigation properties:

        public User? User { get; set; }
        public Game? Game { get; set; }
        public DateTime? DateWished { get; set; }
    }
}
