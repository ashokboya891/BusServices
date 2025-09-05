using BusServcies.DatabaseContext;
using BusServices.Dtos;
using BusServices.IRepositoryContracts;
using BusServices.IServiceContracts;
using BusServices.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics.Eventing.Reader;

namespace BusServices.ServiceContracts
{
    public class EventServcie:IEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEventRepository _EventRepository;
        public EventServcie(IEventRepository EventRepository, ApplicationDbContext con)
        {
            _context = con;
            _EventRepository = EventRepository; // Assuming you have a concrete implementation of IEventRepository
        }


        //public static SqlParameter GetOrderItemsTVP(List<OrderItemDto> items)
        //{
        //    var table = new DataTable();
        //    table.Columns.Add("EventId", typeof(int));
        //    table.Columns.Add("Quantity", typeof(int));
        //    table.Columns.Add("UnitPrice", typeof(decimal));
        //    //table.Columns.Add("UnitPrice", typeof(decimal));
        //    //table.Columns.Add("UnitPrice", typeof(decimal));


        //    foreach (var item in items)
        //    {
        //        table.Rows.Add(item.EventId, item.Quantity, item.UnitPrice);
        //    }

        //    var parameter = new SqlParameter("@OrderItems", table)
        //    {
        //        SqlDbType = SqlDbType.Structured,
        //        TypeName = "dbo.OrderItemType"
        //    };

        //    return parameter;
        //}

        //public async Task<int> UploadOrdersFromExcelFile(IFormFile formfile)
        //{
            //MemoryStream memoryStream = new MemoryStream();
            //await formfile.CopyToAsync(memoryStream);

            //int EventsInserted = 0;

            //using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            //{
            //    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Events"];
            //    int rowCount = worksheet.Dimension.Rows;

            //    for (int row = 2; row <= rowCount; row++) // Skip header
            //    {
            //        string? name = worksheet.Cells[row, 1].Value?.ToString();
            //        string? stockStr = worksheet.Cells[row, 2].Value?.ToString();
            //        string? thresholdStr = worksheet.Cells[row, 3].Value?.ToString();
            //        string? priceStr = worksheet.Cells[row, 4].Value?.ToString();
            //        string? Description = worksheet.Cells[row, 5].Value?.ToString();
            //        string? EventBrandId = worksheet.Cells[row, 6].Value?.ToString();
            //        string? EventTypeId = worksheet.Cells[row, 7].Value?.ToString();




            //        if (!string.IsNullOrWhiteSpace(name) &&
            //            int.TryParse(stockStr, out int currentStock) &&
            //            int.TryParse(thresholdStr, out int threshold) &&
            //            decimal.TryParse(priceStr, out decimal price))
            //        {
            //            var newEvent = new Event
            //            {
            //                Name = name,
            //                CurrentStock = currentStock,
            //                Threshold = threshold,
            //                Price = price,
            //                Description = Description ?? "not added description", // Ensure Description is not null
            //                EventBrandId = int.TryParse(EventBrandId, out int brandId) ? brandId : 0,
            //                EventTypeId = int.TryParse(EventTypeId, out int typeId) ? typeId : 0
            //            };

            //            await _EventRepository.AddEventAsync(newEvent);
            //            EventsInserted++;
            //        }
            //    }
            //}

            //return var a=10;

        //}


        public async Task<EventsToReturn> GetEventByIdAsync(int id)
        {
            var Event = await _EventRepository.GetEventByIdAsync(id);
            if (Event == null) return null;

            return new EventsToReturn
            {
                Id = Event.Id,
                Title = Event.Title,
                Description = Event.Description,
                Price = Event.Price,
                FromPlace=Event.FromPlace,
                ToPlace=Event.ToPlace,
                TravelDate=Event.TravelDate,
                Status=Event.Status,
                Organizer=Event.Organizer,
                ContactInfo=Event.ContactInfo,
                IsActive = Event.IsActive,
                Images=Event.Images.Select(p=>p).ToList(),
                PrimaryImageUrl = Event.Images?.FirstOrDefault(p=>p.IsPrimary)?.Url,
            };
        }

        public async Task<bool> AddEvent(Event Event)
        {
            await _EventRepository.AddEventAsync(Event);
            return await _EventRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateEventAsync(Event Event)
        {
            await _EventRepository.UpdateEventAsync(Event);
            return await _EventRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var Event = await _EventRepository.GetEventByIdAsync(id);
            if (Event == null) return false;

            await _EventRepository.DeleteEventAsync(Event);
            return await _EventRepository.SaveChangesAsync();
        }
        public async Task<BusesEvenMetaDto> GetEventMetaDataAsync()
        {
            var brands = await _context.BusCategory
                .Select(b => new CategoryDto { Id = b.Id, Name = b.Name })
                .ToListAsync();

            var types = await _context.BusTypes
                .Select(t => new TypeDto { Id = t.Id, Name = t.Name })
                .ToListAsync();

            return new BusesEvenMetaDto
            {
                Categories = brands,
                Types = types
            };
    
        }
        public async Task<Event> GetEventEntityByIdAsync(int id)
        {
            return await _context.Events.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Event> GetEventPhotosByIdAsync(int id)
        {
            return await _context.Events.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
        }

    }
}
