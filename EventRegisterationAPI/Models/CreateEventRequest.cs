namespace EventRegisterationAPI.Models
{
    public class CreateEventRequest
    {
        public string Name { get; set; } = string.Empty;

        public int TotalSeats { get; set; }

        public DateTime EventDate { get; set; }
    }
}
