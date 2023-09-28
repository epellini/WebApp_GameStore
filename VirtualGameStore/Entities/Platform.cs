namespace VirtualGameStore.Entities
{
    public class Platform
    {
        // Primary Key:
        public string PlatformId { get; set; } = Guid.NewGuid().ToString();

        // Properties:
        public string? PlatformName { get; set; }

        // Reference navigation property for each dependent entity that has PlatformId as a foreign key:
        public ICollection<Game>? Games { get; set; }
    }
}
