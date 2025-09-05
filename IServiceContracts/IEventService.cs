using BusServices.Dtos;
using BusServcies.Models;
using BusServices.Models;
namespace BusServices.IServiceContracts
{
    public interface IEventService
    {
        //Task<int> UploadOrdersFromExcelFile(IFormFile formfile);
        Task<EventsToReturn> GetEventByIdAsync(int id);
        Task<bool> AddEvent(Event product);
        Task<bool> UpdateEventAsync(Event product);
        Task<bool> DeleteEventAsync(int id);
        //Task<bool> UpdateProductAsync(Product product);
        Task<BusesEvenMetaDto> GetEventMetaDataAsync();
        Task<Event> GetEventEntityByIdAsync(int id);
        Task<Event> GetEventPhotosByIdAsync(int id);
    }
}
