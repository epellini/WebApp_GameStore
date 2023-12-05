using System.ComponentModel.DataAnnotations;

namespace VirtualGameStore.Entities
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int GameId { get; set; }
        public string UserId { get; set; }
        public Game? Game { get; set; }
        public User? User { get; set; }

        [Required(ErrorMessage = "Please enter a review text.")]
        public string? ReviewText { get; set; }
        public string? Status { get; set; } = "Pending";
        public DateTime? ReviewDate { get; set; } = DateTime.Now;
    }
}
