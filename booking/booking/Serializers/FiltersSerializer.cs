using booking.DTO;

namespace booking.Serializers
{
    public class FiltersSerializer
    {
        public List<TypeDTO>? Types { get; set; }
        public List<OsDTO>? Systems { get; set; }
        public List<DepartmentDTO>? Departments { get; set; }
        public List<TagDTO>? Tags { get; set; }
    }
}
