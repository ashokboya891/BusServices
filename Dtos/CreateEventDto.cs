using System.ComponentModel.DataAnnotations;

namespace BusServices.Dtos
{
    public class CreateEventDto
    {
        [Required(ErrorMessage ="TItle must be needed to add event")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description must be needed to add event")]
        public string Description { get; set; }

        [Required(ErrorMessage = "FromPlace must be needed to add event")]
        public string FromPlace { get; set; }

        [Required(ErrorMessage = "ToPlace must be needed to add event")]
        public string ToPlace { get; set; }

        [Required(ErrorMessage = "Travel Date must be needed to add event")]
        public DateTime TravelDate { get; set; }

        [Range(1, 100, ErrorMessage = "Total seats must be between 1 and 100.")]
        public int TotalSeats { get; set; } = 50; // Default value for total seats

        public int AvailableSeats { get; set; } = 0;

        public decimal Price { get; set; } = 0.0m;

        public string Status { get; set; } = "Upcoming"; // Default status
        public string Organizer { get; set; } = string.Empty; // Organizer name
        public string ContactInfo { get; set; } = string.Empty; // Contact information for the event organizer
        public bool IsActive { get; set; } = true; // Indicates if the event is active or not
        public int BusTypeId { get; set; } // Foreign key to BusType
        public int BusCategoryId { set; get; }
    }
}
