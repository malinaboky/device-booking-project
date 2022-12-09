using booking.Entities;

namespace booking.Deserializers
{
    public class SearchRootObject
    {
        public int? Type { get; set; }
        public int? Os { get; set; }
        public int? Department { get; set; }
        public List<Tag>? Tags { get; set; }
    }
}
