namespace VirtualGameStore.Entities
{
    public class PreferredLanguage
    {
        // Primary key property:
        public int PreferredLanguageId { get; set; }

        // Foreign key properties:
        public string? UserId { get; set; }
        public string? LanguageId { get; set; }

        // Reference navigation property to principal entity for each foreign key:
        public User? User { get; set; }
        public Language? Language { get; set; }
    }
}
