using BusServcies.Errors;
using CloudinaryDotNet.Actions;
using SupplyChain.DTOs;

namespace BusServcies.IServiceContracts
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicID);
        //Task<PhotoDto?> AddProductPhotoAndSaveAsync(int productId, IFormFile file);
        Task<string> UploadPhotoAsync(IFormFile file);
        Task<PhotoDto?> AddProductPhotoAndSaveAsync(int productId, IFormFile file);
        Task<ApiResponse> DeleteImageAsync(int imageId);
         Task<List<PhotoDto>?> AddEventPhotosAndSaveAsync(int eventId, List<IFormFile> files);


    }
}
