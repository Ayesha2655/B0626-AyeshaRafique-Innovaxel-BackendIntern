namespace EventRegisterationAPI.Models
{
    public class RegisterUserRequest
    {
        public string UserName { get; set; } = string.Empty;

        public Guid EventId { get; set; }
    }
}
