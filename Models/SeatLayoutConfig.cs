namespace BusServices.Models
{
    public class SeatLayoutConfig
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public List<List<string?>> Pattern { get; set; } = new();
    }

}
