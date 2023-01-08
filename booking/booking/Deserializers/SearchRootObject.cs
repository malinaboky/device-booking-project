using booking.Entities;

namespace booking.Deserializers
{
    public class SearchRootObject
    {
        public string? Name { get; set; }
        public int? Type { get; set; }
        public int? Os { get; set; }
        public int? Department { get; set; }
        public double? MinLen { get; set; }
        public double? MaxLen { get; set; }
        public List<int>? Tags { get; set; }
    }
}
