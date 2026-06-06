namespace EventRegisterationAPI.Models
{
    public class Registration
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public DateTime RegisteredAt { get; set; }

        public bool IsCancelled { get; set; }
    }
}
