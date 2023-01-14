using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

        [JsonProperty("sorttype")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SortType? SortType { get; set; }
    }

    public enum SortType
    {
        [EnumMember(Value = "name")]
        Name,
        [EnumMember(Value = "namereverse")]
        NameReverse,
        [EnumMember(Value = "diagonalmin")]
        DiagonalMin,
        [EnumMember(Value = "diagonalmax")]
        DiagonalMax,
        [EnumMember(Value = "datenew")]
        DateNew,
        [EnumMember(Value = "dateold")]
        DateOld
    }
}
