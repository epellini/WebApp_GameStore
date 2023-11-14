namespace VirtualGameStore.Entities
{
    public class GamePlatform
    {
        // Foreign key properties:
        public int? GameId { get; set; }
        public int? PlatformId { get; set; }

        // Reference navigation property to principal entity for each foreign key:
        public Game? Game { get; set; }
        public Platform? Platform { get; set; }
    }
}
