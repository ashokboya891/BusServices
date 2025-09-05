using BusServices.IRepositoryContracts;
using BusServices.Models;

namespace BusServices.RepositoryContracts
{
    public class EventRepository : IEventRepository
    {
        Task IEventRepository.AddEvent(Event eventtoadd)
        {
            throw new NotImplementedException();
        }

        Task IEventRepository.AddEventAsync(Event evento)
        {
            throw new NotImplementedException();
        }

        Task IEventRepository.DeleteEventAsync(Event eventtodelete)
        {
            throw new NotImplementedException();
        }

        Task<Event> IEventRepository.GetEventByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<bool> IEventRepository.SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        Task IEventRepository.UpdateEventAsync(Event eventtoupdate)
        {
            throw new NotImplementedException();
        }
    }
}
