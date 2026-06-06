namespace EventRegisterationAPI.Models
{
    public class EventResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int TotalSeats { get; set; }

        public DateTime EventDate { get; set; }

        public int TotalRegistrations { get; set; }

        public int AvailableSeats { get; set; }
    }
}
