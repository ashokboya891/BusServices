namespace BusServices.Dtos
{
    public class BusesEvenMetaDto
    {
        public List<CategoryDto> Categories { get; set; }
        public List<TypeDto> Types { get; set; } = new List<TypeDto>();
    }
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
