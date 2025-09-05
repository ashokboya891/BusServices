﻿namespace SupplyChain.DTOs
{
    public class PhotoDto
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public bool IsMain { set; get; }
        public bool IsApproved { get; set; }
        public string PublicId { get; set; }
    }
    public class CreatePhotoUploadDto
    {
        public int Id { get; set; } // EventId or ProductId
        public List<IFormFile> Files { get; set; } = new();
    }
}
