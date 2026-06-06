namespace EventRegisterationAPI.Models
{
    public class CancelRegistrationRequest
    {
        public Guid EventId { get; set; }

        public string UserName { get; set; } = string.Empty;
    }
}
