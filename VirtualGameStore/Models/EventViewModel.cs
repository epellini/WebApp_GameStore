using VirtualGameStore.Entities;

namespace VirtualGameStore.Models
{
    public class EventViewModel
    {
        // Properties:
        public Event? Event { get; set; }

        public List<EventRegistration>? Registrations { get; set; }
    }
}
