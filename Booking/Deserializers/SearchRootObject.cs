using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Database.Deserializers
{
    public class SearchRootObject
    {
        public string? Name { get; set; }
        public long? Type { get; set; }
        public long? Os { get; set; }
        public long? Department { get; set; }
        public double? MinLen { get; set; }
        public double? MaxLen { get; set; }
        public List<long>? Tags { get; set; }

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
