namespace VirtualGameStore.Entities
{
    public class Game
    {
        // Primary key property:
        public string GameId { get; set; } = Guid.NewGuid().ToString();

        // Foreign key properties:
        public string? PlatformId { get; set; }

        // Reference navigation property to principal entitiy for each foreign key:
        public Platform? Platform { get; set; }

        // Properties:
        public string? Name { get; set; }
        public string? Developer { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public double? RetailPrice { get; set; }

    }
}
