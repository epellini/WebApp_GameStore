using VirtualGameStore.Entities;

namespace VirtualGameStore.Models
{
    public class EditProfileViewModel
    {
        public int ProfileId { get; set; }
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool PromoRegistered { get; set; }

        public Photo? CurrentPhoto { get; set; }

        public IFormFile? NewPhoto { get; set; }
    }
}
