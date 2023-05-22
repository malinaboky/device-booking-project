//using booking.Entities;

using Database.Models;

namespace Database.DTO
{
    public class ShortDeviceCardDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Os { get; set; }
        public double? Diagonal { get; set; }
        public DepartmentDTO? Department { get; set; }
        public string? Image { get; set; }
    }
}
