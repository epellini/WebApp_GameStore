namespace VirtualGameStore.Entities
{
    public class Photo
    {
        public int? PhotoId { get; set; }

        // Foreign key property:
        public int? ProfileId { get; set; }

        // Reference navigation to principal entity for foreign key:
        public Profile? Profile { get; set; }

        // Properties:
        public string? AltText { get; set; }
        public byte[]? Image { get; set; }
        public bool? isProfilePic { get; set; }
    }
}
