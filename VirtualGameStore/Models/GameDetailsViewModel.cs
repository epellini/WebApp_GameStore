using VirtualGameStore.Entities;

namespace VirtualGameStore.Models
{
    public class GameDetailsViewModel
    {
        // Properties:

        public Game game { get; set; }
        public int GameId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Developer { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public double? RetailPrice { get; set; }
        public ICollection<Picture>? PictureUrl { get; set; }
        public string? Genres { get; set; }
        public string? Languages { get; set; }
        public string? Platforms { get; set; }
    }
}
