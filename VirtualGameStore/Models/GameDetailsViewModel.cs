using VirtualGameStore.Entities;

namespace VirtualGameStore.Models
{
    public class GameDetailsViewModel
    {
        // Properties:
        public Game Game { get; set; }
        public int GameId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Developer { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public double? RetailPrice { get; set; }
        public List<Picture>? Pictures { get; set; }
        public List<Genre>? Genres { get; set; }
        public List<Language>? Languages { get; set; }
        public List<Platform>? Platforms { get; set; }
        public List<WishedGame>? Wishlist { get; set; }

        public double? AvgRating { get; set; }
        public int? Rating { get; set; }

        public List<Review>? Reviews { get; set; }
        public Review? NewReview { get; set; }
        public int? PendingReviewCount { get; set; }

        public bool? IsSignedIn { get; set; }
    }
}
