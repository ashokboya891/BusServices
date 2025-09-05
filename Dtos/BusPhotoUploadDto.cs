using System.ComponentModel.DataAnnotations;

namespace BusServcies.DTOs
{
    public class BusPhotoUploadDto
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public int Id { get; set; }
    }
}
