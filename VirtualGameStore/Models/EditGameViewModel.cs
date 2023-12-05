using System.ComponentModel.DataAnnotations;
using VirtualGameStore.Entities;

namespace VirtualGameStore.Models
{
    public class EditGameViewModel
    {
        public int GameId { get; set; }

        [Required(ErrorMessage = "Please enter a name.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter a description.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please enter a developer name.")]
        public string? Developer { get; set; }

        [Required(ErrorMessage = "Please select a release date.")]
        public DateTime? ReleaseDate { get; set; }

        [Required(ErrorMessage = "Please enter a retail price.")]
        public double? RetailPrice { get; set; }

        public List<Platform>? AllPlatforms { get; set; }
        public List<Genre>? AllGenres { get; set; }
        public List<Language>? AllLanguages { get; set; }

        public List<GamePlatform>? GamePlatforms { get; set; }
        public List<GameGenre>? GameGenres { get; set; }
        public List<GameLanguage>? GameLanguages { get; set; }

        [Required(ErrorMessage = "Please select at least 1 platform.")]
        public string? Platforms { get; set; }
        [Required(ErrorMessage = "Please select at least 1 genre.")]
        public string? Genres { get; set; }
        [Required(ErrorMessage = "Please select at least 1 language.")]
        public string? Languages { get; set; }

        public Picture? CurrentPicture { get; set; }
        public IFormFile? NewPicture { get; set; }
    }
}
