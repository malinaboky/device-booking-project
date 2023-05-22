namespace Booking.DTO.Record
{
    public class RecordsDeviceCardDTO
    {
        public string Date { get; set; } = null!;
        public List<RecordCard> Records { get; set; } = null!;
    }

    public class RecordCard
    {
        public string UserName { get; set; } = null!;
        public string TimeFrom { get; set; } = null!;
        public string TimeTo { get; set; } = null!;
    }
}
