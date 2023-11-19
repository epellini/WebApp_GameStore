namespace VirtualGameStore.Entities
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int GameId { get; set; }
        public string UserId { get; set; }
        public Game? Game { get; set; }
        public User? User { get; set; }
        public string? ReviewText { get; set; }
        public string? Status { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
