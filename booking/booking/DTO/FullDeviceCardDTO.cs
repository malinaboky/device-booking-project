using booking.Entities;

namespace booking.DTO
{
    public class FullDeviceCardDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public double? Diagonal { get; set; }
        public string? Resolution { get; set; }
        public string? Type { get; set; }
        public string? Os { get; set; }
        public Department? Department { get; set; }
        public string? Info { get; set; }
        public string? Image { get; set; }
    }
}
