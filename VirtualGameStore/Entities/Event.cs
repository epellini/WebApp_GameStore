namespace VirtualGameStore.Entities
{
    public class Event
    {
        public int EventId { get; set; }
        public string? EventName { get; set; }
        public string? EventDescription { get; set; }
        public DateTime? PublishDate { get; set; }
        public string? Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public  string? Location { get; set; }
        public string? Sponsor { get; set; }

        public ICollection<EventRegistration>? EventRegistrations { get; set; }
    }
}
