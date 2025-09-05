namespace BusServices.Dtos
{
    public class UpdateEventDto
     {
            public int Id { get; set; }

            public string Title { get; set; }

            public string Description { get; set; }

            public string FromPlace { get; set; }

            public string ToPlace { get; set; }

            public DateTime TravelDate { get; set; }

            public int TotalSeats { get; set; }

            public int AvailableSeats { get; set; } = 0;

            public decimal Price { get; set; } = 0.0m;

            public string Status { get; set; } = "Upcoming"; // Default status
            public string Organizer { get; set; } = string.Empty; // Organizer name
            public string ContactInfo { get; set; } = string.Empty; // Contact information for the event organizer
            public bool IsActive { get; set; } = true; // Indicates if the event is active or not
            public int BusTypeId { get; set; } // Foreign key to BusType
        }
 }

