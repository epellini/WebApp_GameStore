namespace VirtualGameStore.Entities
{
    public class Picture
    {
        // Primary key property:
        public int PictureId { get; set; }

        // Foreign key property:
        public int? GameId { get; set; }

        // Reference navigation to principal entity for foreign key:
        public Game? Game { get; set; }

        // Properties:
        public string? Title { get; set; }
        public string? AltText { get; set; }
        public bool? IsCoverArt { get; set; }
        public byte[]? Image { get; set; }
    }
}
