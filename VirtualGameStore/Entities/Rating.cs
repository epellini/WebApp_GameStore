namespace VirtualGameStore.Entities
{
    public class Rating
    {
        public int RatingId { get; set; }

        public int GameId { get; set; }
        public string UserId { get; set; }

        public Game? Game { get; set; }
        public User? User { get; set; }


        public int RatingValue { get; set; }
        public DateTime RatingDate { get; set; }
    }
}
