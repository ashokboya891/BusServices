using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BusServices.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

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

        public ICollection<BusImage> Images { get; set; } = new List<BusImage>();
    }

    public class BusType
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } // Seater, Sleeper, Semi-Sleeper, Express

        [StringLength(500)]
        public string Description { get; set; }

        [Range(1, 100)]
        public int DefaultSeats { get; set; } // e.g. 40, 30, etc.

        [StringLength(200)]
        public string Features { get; set; } // AC/Non-AC, WiFi, Charging port

        [Required, StringLength(100)]
        public string Owner { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [Phone]
        public long OwnerContact { get; set; }

        // 🔗 Reverse navigation
        public ICollection<Event> Buses { get; set; } = new List<Event>();

        // 🔗 Relationship with Category
        [ForeignKey("BusCategory")]
        public int BusCategoryId { get; set; }
        public BusCategory BusCategory { get; set; }

        // ✅ New column for JSON layout
        public string SeatLayoutJson { get; set; } = "{}";

        // Strongly-typed property for C# usage
        [NotMapped]
        public SeatLayoutConfig SeatLayout
        {
            get => string.IsNullOrEmpty(SeatLayoutJson)
                ? new SeatLayoutConfig()
                : JsonSerializer.Deserialize<SeatLayoutConfig>(SeatLayoutJson)!;
            set => SeatLayoutJson = JsonSerializer.Serialize(value);
        }
    }

    public class BusCategory
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } // Sleeper, Semi-Sleeper, Express, Deluxe, Courier

        [StringLength(200)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        // 🔗 Reverse nav to BusTypes
        public ICollection<BusType> BusTypes { get; set; } = new List<BusType>();
    }

    public class BusImage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PhotoId { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; } // Optional for Cloudinary deletion
        public bool IsPrimary { get; set; }


        public int EventId { get; set; }
        public Event Events { get; set; }
    }
}
