namespace VirtualGameStore.Entities
{
    public class FavouritePlatform
    {
        // Primary key property:
        public int FavouritePlatformId { get; set; }

        // Foreign key properties:
        public string? UserId { get; set; }
        public int? PlatformId { get; set; }

        // Reference navigation property to principal entity for each foreign key:
        public User? User { get; set; }
        public Platform? Platform { get; set; }
    }
}
