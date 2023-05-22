namespace Booking.DTO.User
{
    public class LoginDTO
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int TimeZone { get; set; }
        public bool RememberMe { get; set; }
    }
}
