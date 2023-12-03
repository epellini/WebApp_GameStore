using System.ComponentModel.DataAnnotations;

namespace VirtualGameStore.Entities
{
    public class Event
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "Please enter an event name.")]
        public string? EventName { get; set; }

        [Required(ErrorMessage = "Please enter an event description.")]
        public string? EventDescription { get; set; }

        public DateTime? PublishDate { get; set; } = DateTime.Today;
        public string? Status { get; set; } = "Upcoming"; // Upcoming, Active, Cancelled, Completed

        [Required(ErrorMessage = "Please select a start date.")]
        public DateTime? StartTime { get; set; }

        [Required(ErrorMessage = "Please select an end date.")]
        public DateTime? EndTime { get; set; }

        [Required(ErrorMessage = "Please enter a location.")]
        public  string? Location { get; set; }
        public string? Sponsor { get; set; }

        public ICollection<EventRegistration>? EventRegistrations { get; set; }
    }
}
