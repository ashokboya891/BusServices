namespace BusServcies.Dtos
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime TravelDate { get; set; }
        public BusTypeDto? BusType { get; set; }
        public BusCategoryDto? BusCategory { get; set; }
        public List<ImageDto> Images { get; set; }
        public string FromPlace { get; set; }
        public string ToPlace { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string Organizer { get; set; }
        public string ContactInfo { get; set; }
        public bool IsActive { get; set; }
        //public bool IsInactive { get; set; }
    }

    public class BusTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SeatLayoutDto? SeatLayout { get; set; }

    }
    public class SeatLayoutDto
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public List<List<string?>> Pattern { get; set; } = new();
    };


    public class BusCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ImageDto
    {
        public int PhotoId { get; set; }
        public string Url { get; set; }
        public bool IsMain { set; get; }
    }

}
