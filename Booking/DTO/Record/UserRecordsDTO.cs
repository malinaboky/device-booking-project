namespace Booking.DTO.Record
{
    public class UserRecordsDTO
    {

        public long Id { get; set; }
        public DeviceInfo Device { get; set; } = null!;
        public string Date { get; set; } = null!;
        public string TimeFrom { get; set; } = null!;
        public string TimeTo { get; set; } = null!;
        public bool Booked { get; set; }
    }

    public class DeviceInfo
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ImgPath { get; set; }
    }
}
