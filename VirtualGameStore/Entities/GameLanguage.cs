namespace VirtualGameStore.Entities
{
    public class GameLanguage
    {
        // Primary key property:
        public int GameLanguageId { get; set; }

        // Foreign key properties:
        public int? GameId { get; set; }
        public string? LanguageId { get; set; }

        // Reference navigation property to principal entity for each foreign key:
        public Game? Game { get; set; }
        public Language? Language { get; set; }
    }
}
