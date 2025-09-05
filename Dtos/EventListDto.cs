namespace BusServices.Dtos
{
    public class EventListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FromPlace { get; set; }
        public string ToPlace { get; set; }
        public DateTime TravelDate { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string Organizer { get; set; }
        public string ContactInfo { get; set; }
        public bool IsActive { get; set; }
        public BusTypeDtoR? BusType { get; set; }
        public BusCategoryDtoR? BusCategory { get; set; }
        public List<ImageDtoR> Images { get; set; }
    }

    public class BusTypeDtoR
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Features { get; set; }
    }

    public class BusCategoryDtoR
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ImageDtoR
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsPrimary { get; set; }
    }

}
