namespace EventRegisterationAPI.Models
{
    public class EventModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int TotalSeats { get; set; }

        public DateTime EventDate { get; set; }

        public List<Registration> Registrations { get; set; } = new();
    }
}
