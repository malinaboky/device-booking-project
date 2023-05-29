namespace Booking.DTO.Record
{
    public class CalendarRecordsOfDevice : TimeInfoOfRecord
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public bool Booked { get; set; }
        public long DeviceId { get; set; }
    }
}
