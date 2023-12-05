namespace VirtualGameStore.Entities
{
    public class EventRegistration
    {
        public int EventRegistrationId { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
        public Event? Event { get; set; }
        public User? User { get; set; }
    }
}
