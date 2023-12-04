namespace VirtualGameStore.Entities
{
    public class Game
    {
        // Primary key property:
        public int GameId { get; set; }

        // Properties:
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Developer { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public double? RetailPrice { get; set; }

        // Reference navigation properties to dependent entities that have GameId as a foreign key:
        public ICollection<GameGenre>? Genres { get; set; }
        public ICollection<GameLanguage>? Languages { get; set; }
        public ICollection<GamePlatform>? Platforms { get; set; }
        public ICollection<Picture>? Pictures { get; set; }
        public ICollection<WishedGame>? WishedGames { get; set; }
        public ICollection<Rating>? Ratings { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
