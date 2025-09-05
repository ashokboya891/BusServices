using BusServices.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusServices.Dtos
{
    public class EventWithPhotosDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FromPlace { get; set; }
        public string ToPlace { get; set; }
        public DateTime TravelDate { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string Organizer { get; set; }
        public string ContactInfo { get; set; }
        public bool IsActive { get; set; } = true;

        // Foreign keys
        public int BusTypeId { get; set; }
        public BusType BusType { get; set; }

        public int BusCategoryId { get; set; }
        public BusCategory BusCategory { get; set; }
        public string PrimaryImageUrl { get; set; }
        public ICollection<BusImage> Images { get; set; } = new List<BusImage>();

    }
}
