namespace BusServices.Dtos
{
  

    public record EventDtoBook(
    int Id,
    string Title,
    string Description,
    string FromPlace,
    string ToPlace,
    DateTime TravelDate,
    int TotalSeats,
    decimal Price,
    string Status,
    string Organizer,
    string ContactInfo,
    bool IsActive,
    BusTypeDtoBook? BusType,
    BusCategoryDtoBook? BusCategory,
    List<ImageDtoBook> Images
);

    public class BusTypeDtoBook
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public SeatLayoutDtoBook? SeatLayout { set; get; }
    };

    public record SeatLayoutDtoBook(
        int Rows,
        int Cols,
        List<List<string?>> Pattern
    );

    public record BusCategoryDtoBook(int Id, string Name);

    public record ImageDtoBook(int PhotoId, string Url, bool IsMain);

}
