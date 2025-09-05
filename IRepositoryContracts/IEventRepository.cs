using BusServices.Models;

namespace BusServices.IRepositoryContracts
{
    public interface IEventRepository
    {
        Task AddEventAsync(Event evento);
        Task<Event> GetEventByIdAsync(int id);
        Task AddEvent(Event eventtoadd);
        Task UpdateEventAsync(Event eventtoupdate);
        Task DeleteEventAsync(Event eventtodelete);
        Task<bool> SaveChangesAsync();
    }
}
