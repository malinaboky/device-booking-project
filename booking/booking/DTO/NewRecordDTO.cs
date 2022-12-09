namespace booking.DTO
{
    public class NewRecordDTO
    {
        public int DeviceId { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
        public int DepartmentId { get; set; }
    }
}
