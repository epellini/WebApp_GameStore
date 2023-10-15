namespace VirtualGameStore.Entities
{
    public class Genre
    {
        // Primary Key:
        public int GenreId { get; set; }

        // Properties:
        public string? GenreName { get; set; }

        // Reference navigation property for each dependent entity that has GenreId as a foreign key:
        public ICollection<GameGenre>? Games { get; set; }
        public ICollection<FavouriteGenre>? Users { get; set; }
    }
}
