namespace Booking.DTO.Record
{
    public class ConvertTimeRecordDTO
    {
        public DateOnly Date { get; set; }
        public TimeOnly TimeFrom { get; set; }
        public TimeOnly TimeTo { get; set; }
    }
}
