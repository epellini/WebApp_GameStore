namespace VirtualGameStore.Entities
{
    public class FavouriteGenre
    {
        // Primary key property:
        public int FavouriteGenreId { get; set; }

        // Foreign key properties:
        public string? UserId { get; set; }
        public int? GenreId { get; set; }

        // Reference navigation property to principal entity for each foreign key:
        public User? User { get; set; }
        public Genre? Genre { get; set; }
    }
}
