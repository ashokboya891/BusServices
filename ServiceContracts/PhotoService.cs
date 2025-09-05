using API.Helpers;
using BusServcies.DatabaseContext;
using BusServcies.Errors;
using BusServcies.IServiceContracts;
using BusServcies.Models;
using BusServices.Models;
using CloudinaryDotNet;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SupplyChain.DTOs;

namespace BusServcies.Services
{
    public class PhotoService: IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ApplicationDbContext _context;
        public PhotoService(IOptions<CloudinarySettings> config,ApplicationDbContext context)
        {
            var acc = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
            _context = context;
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = "BusBooking"
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);

            }
            return uploadResult;

        }
        public async Task<List<PhotoDto>?> AddEventPhotosAndSaveAsync(int eventId, List<IFormFile> files)
        {
            if (files == null || files.Count == 0) return null;

            // Fetch event and include images
            var eventEntity = await _context.Events
                .Include(e => e.Images)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventEntity == null) return null;

            var photoDtos = new List<PhotoDto>();

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                // Upload to Cloudinary
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Transformation = new Transformation()
                        .Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = "EventBooking"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null) continue;

                // Add photo entity
                var photo = new BusImage
                {
                    Url = uploadResult.SecureUrl.AbsoluteUri,
                    PublicId = uploadResult.PublicId,
                    IsPrimary = !eventEntity.Images.Any() // make first uploaded as primary
                };

                eventEntity.Images.Add(photo);

                photoDtos.Add(new PhotoDto
                {
                    Id = photo.PhotoId,
                    Url = photo.Url,
                    PublicId = photo.PublicId,
                    IsMain = photo.IsPrimary
                });
            }

            // Save all changes
            var saved = await _context.SaveChangesAsync() > 0;
            return saved ? photoDtos : null;
        }


        public async Task<PhotoDto?> AddProductPhotoAndSaveAsync(int productId, IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            // Fetch product and include photos
            var product = await _context.Events
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null) return null;

            // Upload to Cloudinary
            var uploadResult = new ImageUploadResult();
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder = "BusBooking"
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null) return null;

            // Add to product entity
            var photo = new BusImage
            {
                Url = uploadResult.SecureUrl.AbsoluteUri,
                PublicId = uploadResult.PublicId
            };

            product.Images.Add(photo);

            // Save to DB
            var saved = await _context.SaveChangesAsync() > 0;
            if (!saved) return null;

            return new PhotoDto
            {
                Id = photo.PhotoId,
                Url = photo.Url,
                IsMain = photo.IsPrimary,
                PublicId = photo.PublicId
            };
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicID)
        {
            var deleteParams = new DeletionParams(publicID);
            return await _cloudinary.DestroyAsync(deleteParams);
        }

  
        public async Task<string> UploadPhotoAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(800).Crop("fill")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                    throw new Exception(uploadResult.Error.Message);

                return uploadResult.SecureUrl.AbsoluteUri;
            }

            return null;
        }
        public async Task<ApiResponse> DeleteImageAsync(int imageId)
        {
            var image = await _context.BusImages.FirstOrDefaultAsync(i => i.PhotoId == imageId);

            if (image == null)
                return new ApiResponse(404, "Image not found.");

            if (image.IsPrimary) // ❌ Prevent deletion of main image
                return new ApiResponse(400, "Cannot delete the main photo.");

            _context.BusImages.Remove(image);
            await _context.SaveChangesAsync();

            return new ApiResponse(200, "Image deleted successfully.");
        }

    }
}
