namespace VirtualGameStore.Entities
{
    public class Language
    {
        // Primary Key:
        public string LanguageId { get; set; }

        // Properties:
        public string? LanguageName { get; set; }

        // Reference navigation property for each dependent entity that has PlatformId as a foreign key:
        public ICollection<GameLanguage>? Games { get; set; }
        public ICollection<PreferredLanguage>? Users { get; set; }
    }
}
