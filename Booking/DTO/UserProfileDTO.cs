namespace Database.DTO
{
    public class UserProfileDTO
    {
        public long Id { get; set; }
        public string? Firstname { get; set; }
        public string? Secondname { get; set; }
        public string? ConnectLink { get; set; }
        public string Username { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? Image { get; set; }
    }
}