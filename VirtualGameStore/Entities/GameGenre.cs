namespace VirtualGameStore.Entities
{
    public class GameGenre
    {
        // Primary key property:
        public int GameGenreId { get; set; }

        // Foreign key properties:
        public int? GameId { get; set; }
        public int? GenreId { get; set; }

        // Reference navigation property to principal entity for each foreign key:
        public Game? Game { get; set; }
        public Genre? Genre { get; set; }
    }
}
