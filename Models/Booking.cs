using System.ComponentModel.DataAnnotations;

namespace BusServices.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        public int SeatNumner { set; get; }
        public  DateTime BookingDate { get; set; }
    }
}
