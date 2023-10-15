namespace VirtualGameStore.Entities
{
    public class Profile
    {
        // Primary key property:
        public int ProfileId { get; set; }

        // Foreign key properties:
        public string? UserId { get; set; }

        // Reference navigation property to principal entity for each foreign key:
        public User? User { get; set; }

        // Properties:
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool? PromoRegistered { get; set; }
    }
}
