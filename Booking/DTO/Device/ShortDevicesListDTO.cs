using Database.DTO;
using Database.Models;

namespace Booking.DTO.Device
{
    public class ShortDevicesListDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Os { get; set; }
        public double? Diagonal { get; set; }
        public DepartmentDTO? Department { get; set; }
        public string? Image { get; set; }
    }
}
