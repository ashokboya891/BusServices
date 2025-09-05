namespace BusServices.Models
{
    public class User
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Email { set; get; }
        public ICollection<Booking> Bookings { get; set; }

    }
}
