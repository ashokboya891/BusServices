using BusServcies.DatabaseContext;
using BusServcies.Dtos;
using BusServcies.Errors;
using BusServcies.Helpers;
using BusServcies.IServiceContracts;
using BusServices.Dtos;
using BusServices.IServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.DTOs;
using System.Linq;

namespace BusServices.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UserController:ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly ApplicationDbContext _context;
        private readonly IEventService _eventService;
        public UserController(ApplicationDbContext context, IPhotoService photoService, IEventService events)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _photoService = photoService;
            _eventService = events;
        }

        [HttpGet("[action]")]
        //[Cached(600)]
        public async Task<IActionResult> GetAllEvents([FromQuery] BusQueryParams busQueryParams)
        {
            var query = _context.Events
                .Include(e => e.BusType)
                .Include(i => i.Images)
                .Include(p => p.BusCategory)
                .AsQueryable();

            // 🔍 Searching (Title/Description/Organizer)
            if (!string.IsNullOrEmpty(busQueryParams.Search))
                query = query.Where(e =>
                    e.Title.Contains(busQueryParams.Search) ||
                    e.Description.Contains(busQueryParams.Search) ||
                    e.Organizer.Contains(busQueryParams.Search));

            // 📍 Filtering
            if (!string.IsNullOrEmpty(busQueryParams.FromPlace))
                query = query.Where(e => e.FromPlace.Contains(busQueryParams.FromPlace));
            if (!string.IsNullOrEmpty(busQueryParams.ToPlace))
                query = query.Where(e => e.ToPlace.Contains(busQueryParams.ToPlace));

            if (busQueryParams.BusTypeId.HasValue)
                query = query.Where(e => e.BusTypeId == busQueryParams.BusTypeId);

            if (busQueryParams.BusCategoryId.HasValue)
                query = query.Where(e => e.BusCategoryId == busQueryParams.BusCategoryId);

            // 📅 Date filtering
            if (busQueryParams.FromDate.HasValue && busQueryParams.ToDate.HasValue)
            {
                var fromDate = DateTime.SpecifyKind(busQueryParams.FromDate.Value.Date, DateTimeKind.Utc);
                var toDate = DateTime.SpecifyKind(busQueryParams.ToDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
                query = query.Where(e => e.TravelDate >= fromDate && e.TravelDate <= toDate);
            }
            else
            {
                if (busQueryParams.FromDate.HasValue)
                {
                    var fromDate = DateTime.SpecifyKind(busQueryParams.FromDate.Value.Date, DateTimeKind.Utc);
                    query = query.Where(e => e.TravelDate >= fromDate);
                }
                if (busQueryParams.ToDate.HasValue)
                {
                    var toDate = DateTime.SpecifyKind(busQueryParams.ToDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
                    query = query.Where(e => e.TravelDate <= toDate);
                }
            }

            // ↕ Sorting
            query = busQueryParams.SortBy?.ToLower() switch
            {
                "price" => busQueryParams.SortDirection == "desc" ? query.OrderByDescending(e => e.Price) : query.OrderBy(e => e.Price),
                "seats" => busQueryParams.SortDirection == "desc" ? query.OrderByDescending(e => e.AvailableSeats) : query.OrderBy(e => e.AvailableSeats),
                "title" => busQueryParams.SortDirection == "desc" ? query.OrderByDescending(e => e.Title) : query.OrderBy(e => e.Title),
                _ => busQueryParams.SortDirection == "desc" ? query.OrderByDescending(e => e.TravelDate) : query.OrderBy(e => e.TravelDate)
            };

            // 📑 Pagination
            var totalRecords = await query.CountAsync();

            var events = await query
                .Skip((busQueryParams.Page - 1) * busQueryParams.PageSize)
                .Take(busQueryParams.PageSize)
                .Select(e => new EventListDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    FromPlace = e.FromPlace,
                    ToPlace = e.ToPlace,
                    TravelDate = e.TravelDate,
                    TotalSeats = e.TotalSeats,
                    AvailableSeats = e.AvailableSeats,
                    Price = e.Price,
                    Status = e.Status,
                    Organizer = e.Organizer,
                    ContactInfo = e.ContactInfo,
                    IsActive = e.IsActive,
                    BusType = e.BusType != null ? new BusTypeDtoR
                    {
                        Id = e.BusType.Id,
                        Name = e.BusType.Name,
                        Description = e.BusType.Description,
                        Features = e.BusType.Features
                    } : null,
                    BusCategory = e.BusCategory != null ? new BusCategoryDtoR
                    {
                        Id = e.BusCategory.Id,
                        Name = e.BusCategory.Name,
                        Description = e.BusCategory.Description
                    } : null,
                    Images = e.Images.Select(img => new ImageDtoR
                    {
                        Id = img.PhotoId,
                        Url = img.Url,
                        IsPrimary = img.IsPrimary
                    }).ToList()
                })
                .ToListAsync();

            if (!events.Any())
                return NotFound(new ApiResponse(404, "No events found."));

            return Ok(new
            {
                TotalRecords = totalRecords,
                Page = busQueryParams.Page,
                PageSize = busQueryParams.PageSize,
                Data = events
            });
        }

        [HttpGet("[action]")]
        //[Cached(600)]
        public async Task<IActionResult> GetEventById([FromQuery] int id)
        {
            var existingEvent = await _context.Events
                .Include(e => e.BusType)
                .Include(o => o.BusCategory)
                .Include(i => i.Images)
                .Where(e => e.Id == id)
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    TravelDate = e.TravelDate,
                    FromPlace = e.FromPlace,
                    ToPlace = e.ToPlace,
                    TotalSeats = e.TotalSeats,
                    AvailableSeats = e.AvailableSeats,
                    Price = e.Price,
                    Status = e.Status,
                    Organizer = e.Organizer,
                    ContactInfo = e.ContactInfo,
                    IsActive = e.IsActive,

                    BusType = e.BusType != null ? new BusTypeDto
                    {
                        Id = e.BusType.Id,
                        Name = e.BusType.Name
                    } : null,
                    BusCategory = e.BusCategory != null ? new BusCategoryDto
                    {
                        Id = e.BusCategory.Id,
                        Name = e.BusCategory.Name
                    } : null,
                    Images = e.Images.Select(img => new ImageDto
                    {
                        PhotoId = img.PhotoId,
                        Url = img.Url,
                        IsMain = img.IsPrimary
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (existingEvent == null)
                return NotFound(new ApiResponse(404, "Event not found."));

            return Ok(existingEvent);
        }

        [HttpGet("events/{eventId:int}/for-booking")]
        public async Task<IActionResult> GetEventForBooking(int eventId)
        {
            var existingEvent = await _context.Events
    .Include(e => e.BusType)
    .Include(e => e.BusCategory)
    .Include(e => e.Images)
    .Where(e => e.Id == eventId)
    .Select(e => new EventDto
    {
        Id = e.Id,
        Title = e.Title,
        Description = e.Description,
        FromPlace = e.FromPlace,
        ToPlace = e.ToPlace,
        TravelDate = e.TravelDate,
        TotalSeats = e.TotalSeats,
        AvailableSeats = e.AvailableSeats,
        Price = e.Price,
        Status = e.Status,
        Organizer = e.Organizer,
        ContactInfo = e.ContactInfo,
        IsActive = e.IsActive,
        BusType = e.BusType != null ? new BusTypeDto
        {
            Id = e.BusType.Id,
            Name = e.BusType.Name,
            SeatLayout = e.BusType.SeatLayout == null ? null : new SeatLayoutDto
            {
                Rows = e.BusType.SeatLayout.Rows,
                Cols = e.BusType.SeatLayout.Cols,
                Pattern = e.BusType.SeatLayout.Pattern
            }
        } : null,
        BusCategory = e.BusCategory != null ? new BusCategoryDto
        {
            Id = e.BusCategory.Id,
            Name = e.BusCategory.Name
        } : null,
        Images = e.Images.Select(img => new ImageDto
        {
            PhotoId = img.PhotoId,
            Url = img.Url,
            IsMain = img.IsPrimary
        }).ToList()
    })
    .FirstOrDefaultAsync();
            //var existingEvent = await _context.Events
            //    .Include(e => e.BusType)
            //    .Include(e => e.BusCategory)
            //    .Include(e => e.Images)
            //    .Where(e => e.Id == eventId)
            //    .Select(e => new EventDto(
            //        Id: e.Id,
            //        Title: e.Title,
            //        Description: e.Description,
            //        FromPlace: e.FromPlace,
            //        ToPlace: e.ToPlace,
            //        TravelDate: e.TravelDate,
            //        TotalSeats: e.TotalSeats,
            //        Price: e.Price,
            //        Status: e.Status,
            //        Organizer: e.Organizer,
            //        ContactInfo: e.ContactInfo,
            //        IsActive: e.IsActive,
            //        BusType: e.BusType != null ? new BusTypeDto(
            //            Id: e.BusType.Id,
            //            Name: e.BusType.Name,
            //            SeatLayout: e.BusType.SeatLayout == null ? null : new SeatLayoutDto
            //            {
            //                Rows = e.BusType.SeatLayout.Rows,
            //                Cols = e.BusType.SeatLayout.Cols,
            //                Pattern = e.BusType.SeatLayout.Pattern // Ensure this is a List<List<string?>> in your entity
            //            }
            //            }:null,// Make sure this deserializes from JSON
            //        ) : null,
            //        BusCategory: e.BusCategory != null ? new BusCategoryDtoBook(
            //            id: e.BusCategory.Id,
            //            Name: e.BusCategory.Name
            //        ) : null,
            //        Images: e.Images.Select(img => new ImageDto(
            //            pho: img.PhotoId,
            //            Url: img.Url,
            //            IsMain: img.IsPrimary
            //        )).ToList()
            //    ))
            //    .FirstOrDefaultAsync();

            if (existingEvent == null)
                return NotFound(new ApiResponse(404, "Event not found."));

            return Ok(existingEvent);
        }



    }
}
