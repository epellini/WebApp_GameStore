namespace VirtualGameStore.Entities
{
    public class Platform
    {
        // Primary Key:
        public int PlatformId { get; set; }

        // Properties:
        public string? PlatformName { get; set; }

        // Reference navigation property for each dependent entity that has PlatformId as a foreign key:
        public ICollection<GamePlatform>? Games { get; set; }
        public ICollection<FavouritePlatform>? Users { get; set; }
    }
}
