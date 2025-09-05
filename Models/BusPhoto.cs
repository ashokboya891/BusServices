using BusServices.Models;
using System.ComponentModel.DataAnnotations;

namespace BusServcies.Models
{
    public class BusPhoto
    {
        [Key]
        public int PhotoId { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; } // Optional for Cloudinary deletion
        public bool IsPrimary { get; set; }


        public int EventId { get; set; }
        public Event Events { get; set; }
    }
}
