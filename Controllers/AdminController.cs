using BusServcies.DatabaseContext;
using BusServcies.Dtos;
using BusServcies.DTOs;
using BusServcies.Errors;
using BusServcies.Helpers;
using BusServcies.IServiceContracts;
using BusServices.Dtos;
using BusServices.IServiceContracts;
using BusServices.Models;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SupplyChain.DTOs;
using System;

namespace BusServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly ApplicationDbContext _context;
        private readonly IEventService _eventService;
        public AdminController(ApplicationDbContext context,IPhotoService photoService,IEventService events)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _photoService = photoService;
            _eventService = events;
        }

        [HttpPost("add-photo")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PhotoDto>> AddPhoto([FromForm] BusPhotoUploadDto dto)
        {
            var result = await _photoService.AddProductPhotoAndSaveAsync(dto.Id, dto.File);
            if (result == null)
                return BadRequest("Failed to upload and save photo");

            return Ok(result);
        }


        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is missing.");

            try
            {
                var imageUrl = await _photoService.UploadPhotoAsync(file);
                return Ok(imageUrl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Image upload failed: {ex.Message}");
            }
        }
        //[HttpPost("upload-image")]
        //public async Task<IActionResult> UploadImage(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded.");

        //    var uploadsFolder = Path.Combine(Environment.CurrentDirectory); // inject IWebHostEnvironment _environment
        //    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        //    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    var imageUrl = "/" + uniqueFileName;  // URL relative to wwwroot

        //    return Ok(imageUrl);
        //}


        //// ✅ Create New Event
        //[HttpPost("[action]")]
        //public async Task<IActionResult> CreateNewEvent([FromBody] CreateEventDto eventdto)
        //{
        //    if (string.IsNullOrEmpty(eventdto.Title) || eventdto.TravelDate == null)
        //        return BadRequest(new ApiResponse(400, "Invalid event data."));

        //    var defaultImageUrl = "/bus.jpg";  // Path relative to wwwroot

        //    var newEvent = new Event
        //    {
        //        Title = eventdto.Title,
        //        Description = eventdto.Description,
        //        FromPlace = eventdto.FromPlace,
        //        ToPlace = eventdto.ToPlace,
        //        TravelDate = eventdto.TravelDate.ToUniversalTime(), // ✅ convert to UTC
        //        TotalSeats = eventdto.TotalSeats,
        //        AvailableSeats = eventdto.AvailableSeats,
        //        Price = eventdto.Price,
        //        Status = eventdto.Status,
        //        Organizer = eventdto.Organizer,
        //        ContactInfo = eventdto.ContactInfo,
        //        IsActive = eventdto.IsActive,
        //        BusTypeId = eventdto.BusTypeId,
        //        BusCategoryId = eventdto.BusCategoryId,
        //        Images = new List<BusImage> {
        //           new BusImage {
        //            Url= string.IsNullOrEmpty(eventdto.ImageUrl) ? defaultImageUrl : eventdto.ImageUrl,
        //            IsPrimary = false,
        //            PublicId=eventdto.PublicId
        //           }
        //         }
        //    };

        //    try
        //    {
        //        _context.Events.Add(newEvent);
        //        await _context.SaveChangesAsync();
        //        return Ok(new ApiResponse(200, "Event created successfully."));
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        // Log exception details (not shown here)
        //        return StatusCode(500, new ApiResponse(500, "Database error. Event might already exist."));
        //    }
        //}

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateNewEvent([FromBody] CreateEventDto eventdto)
        {
            if (string.IsNullOrEmpty(eventdto.Title) || eventdto.TravelDate == null)
                return BadRequest(new ApiResponse(400, "Invalid event data."));

            var defaultImageUrl = "/bus.jpg";  // Path relative to wwwroot

            var newEvent = new Event
            {
                Title = eventdto.Title,
                Description = eventdto.Description,
                FromPlace = eventdto.FromPlace,
                ToPlace = eventdto.ToPlace,
                TravelDate = eventdto.TravelDate.ToUniversalTime(),
                TotalSeats = eventdto.TotalSeats,
                AvailableSeats = eventdto.AvailableSeats,
                Price = eventdto.Price,
                Status = eventdto.Status,
                Organizer = eventdto.Organizer,
                ContactInfo = eventdto.ContactInfo,
                IsActive = eventdto.IsActive,
                BusTypeId = eventdto.BusTypeId,
                BusCategoryId = eventdto.BusCategoryId
            };

            try
            {
                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();

                // return id and message ✅
                return Ok(new
                {
                    id = newEvent.Id,
                    message = "Event created successfully."
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ApiResponse(500, "Database error. Event might already exist."));
            }
        }
        [HttpPost("AddEventPhotos")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddEventPhotos([FromForm] int eventId, [FromForm] List<IFormFile> files)
        {
            var result = await _photoService.AddEventPhotosAndSaveAsync(eventId, files);
            if (result == null || !result.Any())
                return BadRequest("Failed to upload and save photos");

            return Ok(result);
        }




        [HttpPut("[action]")]
        //[Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateEvent([FromQuery] int Id, [FromBody] UpdateEventDto eventdto)
        {
            if (eventdto == null || eventdto.Id != Id)
                return BadRequest(new ApiResponse(400, "Invalid event data."));

            var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.Id == Id);
            if (existingEvent == null)
                return NotFound(new ApiResponse(404, "Event not found."));

            existingEvent.Title = eventdto.Title;
            existingEvent.Description = eventdto.Description;
            existingEvent.FromPlace = eventdto.FromPlace;
            existingEvent.ToPlace = eventdto.ToPlace;
            existingEvent.TravelDate = eventdto.TravelDate;
            existingEvent.TotalSeats = eventdto.TotalSeats;
            existingEvent.AvailableSeats = eventdto.AvailableSeats; // ✅ Don’t overwrite to TotalSeats always
            existingEvent.Price = eventdto.Price;
            existingEvent.Status = eventdto.Status;
            existingEvent.Organizer = eventdto.Organizer;
            existingEvent.ContactInfo = eventdto.ContactInfo;
            existingEvent.IsActive = eventdto.IsActive;
            existingEvent.BusTypeId = eventdto.BusTypeId;

            _context.Events.Update(existingEvent);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse(200, "Event updated successfully."));
        }



        // ✅ Delete Event
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteEvent([FromQuery] int Id)
        {
            var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.Id == Id);
            if (existingEvent == null)
                return NotFound(new ApiResponse(404, "Event not found."));

            _context.Events.Remove(existingEvent);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse(200, "Event deleted successfully."));
        }

        // ✅ Get All Events (with Pagination, Filtering, Sorting, Searching)
        //[HttpGet("[action]")]
        //public async Task<IActionResult> GetAllEvents([FromQuery]BusQueryParams busQueryParams )
        //{
        //    var query = _context.Events.Include(e => e.BusType).Include(i=>i.Images).Include(p=>p.BusCategory).AsQueryable();

        //    // 🔍 Searching (Title/Description/Organizer)
        //    if (!string.IsNullOrEmpty(busQueryParams.Search))
        //        query = query.Where(e =>
        //            e.Title.Contains(busQueryParams.Search) ||
        //            e.Description.Contains(busQueryParams.Search) ||
        //            e.Organizer.Contains(busQueryParams.Search));

        //    // 📍 Filtering (By FromPlace & ToPlace)
        //    if (!string.IsNullOrEmpty(busQueryParams.FromPlace))
        //        query = query.Where(e => e.FromPlace.Contains(busQueryParams.FromPlace));
        //    if (!string.IsNullOrEmpty(busQueryParams.ToPlace))
        //        query = query.Where(e => e.ToPlace.Contains(busQueryParams.ToPlace));

        //    // 🚌 Filtering (By BusType)
        //    if (busQueryParams.BusTypeId.HasValue)
        //        query = query.Where(e => e.BusTypeId == busQueryParams.BusTypeId);

        //    // 📅 Filtering (By buscategory)
        //    if (busQueryParams.BusCategoryId.HasValue)
        //        query = query.Where(e => e.BusCategoryId == busQueryParams.BusCategoryId);

        //    // ↕ Sorting
        //    query = busQueryParams.SortBy?.ToLower() switch
        //    {
        //        "price" => busQueryParams.SortDirection == "desc" ? query.OrderByDescending(e => e.Price) : query.OrderBy(e => e.Price),
        //        "seats" => busQueryParams.SortDirection == "desc" ? query.OrderByDescending(e => e.AvailableSeats) : query.OrderBy(e => e.AvailableSeats),
        //        "title" => busQueryParams.SortDirection == "desc" ? query.OrderByDescending(e => e.Title) : query.OrderBy(e => e.Title),
        //        _ => busQueryParams.SortDirection == "desc" ? query.OrderByDescending(e => e.TravelDate) : query.OrderBy(e => e.TravelDate)
        //    };

        //    //if (busQueryParams.FromDate.HasValue && busQueryParams.ToDate.HasValue)
        //    //{
        //    //    var fromDate = busQueryParams.FromDate.Value.Date; // start of day
        //    //    var toDate = busQueryParams.ToDate.Value.Date.AddDays(1).AddTicks(-1); // end of day
        //    //    query = query.Where(e => e.TravelDate >= fromDate && e.TravelDate <= toDate);
        //    //}
        //    //else
        //    //{
        //    //    if (busQueryParams.FromDate.HasValue)
        //    //    {
        //    //        var fromDate = busQueryParams.FromDate.Value.Date;
        //    //        query = query.Where(e => e.TravelDate >= fromDate);
        //    //    }
        //    //    if (busQueryParams.ToDate.HasValue)
        //    //    {
        //    //        var toDate = busQueryParams.ToDate.Value.Date.AddDays(1).AddTicks(-1);
        //    //        query = query.Where(e => e.TravelDate <= toDate);
        //    //    }
        //    //}

        //    if (busQueryParams.FromDate.HasValue && busQueryParams.ToDate.HasValue)
        //    {
        //        var fromDate = DateTime.SpecifyKind(busQueryParams.FromDate.Value.Date, DateTimeKind.Utc);
        //        var toDate = DateTime.SpecifyKind(busQueryParams.ToDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        //        query = query.Where(e => e.TravelDate >= fromDate && e.TravelDate <= toDate);
        //    }
        //    else
        //    {
        //        if (busQueryParams.FromDate.HasValue)
        //        {
        //            var fromDate = DateTime.SpecifyKind(busQueryParams.FromDate.Value.Date, DateTimeKind.Utc);
        //            query = query.Where(e => e.TravelDate >= fromDate);
        //        }
        //        if (busQueryParams.ToDate.HasValue)
        //        {
        //            var toDate = DateTime.SpecifyKind(busQueryParams.ToDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
        //            query = query.Where(e => e.TravelDate <= toDate);
        //        }
        //    }

        //    // 📑 Pagination
        //    var totalRecords = await query.CountAsync();
        //    var events = await query.Skip((busQueryParams.Page - 1) * busQueryParams.PageSize).Take(busQueryParams.PageSize).ToListAsync();

        //    if (!events.Any())
        //        return NotFound(new ApiResponse(404, "No events found."));

        //    return Ok(new
        //    {
        //        TotalRecords = totalRecords,
        //        Page = busQueryParams.Page,
        //        PageSize = busQueryParams.PageSize,
        //        Data = events
        //    });
        //}

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
                    FromPlace= e.FromPlace,
                    ToPlace= e.ToPlace,
                    TotalSeats= e.TotalSeats,
                    AvailableSeats= e.AvailableSeats,
                    Price= e.Price,
                    Status= e.Status,
                    Organizer= e.Organizer,
                    ContactInfo= e.ContactInfo,
                    IsActive= e.IsActive,

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
                       IsMain= img.IsPrimary
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (existingEvent == null)
                return NotFound(new ApiResponse(404, "Event not found."));

            return Ok(existingEvent);
        }


        // ✅ Update Available Seats
        [HttpPut("UpdateAvailableSeats")]
        public async Task<IActionResult> UpdateAvailableSeats([FromQuery] int Id, [FromQuery] int seatsToReduce)
        {
            var busEvent = await _context.Events.FindAsync(Id);
            if (busEvent == null)
                return NotFound("Event not found.");

            if (busEvent.AvailableSeats < seatsToReduce)
                return BadRequest("Not enough available seats.");

            busEvent.AvailableSeats -= seatsToReduce;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Seats updated successfully",
                EventId = Id,
                RemainingSeats = busEvent.AvailableSeats
            });
        }

        //[Cached(600)]
        [HttpGet("[action]")]
        public IActionResult GetBusTypes()
        {
            return Ok(_context.BusTypes.ToList());
        }
        [HttpGet("[action]")]
        //[Cached(600)]
        public IActionResult GetBusCategoryies()
        {
            return Ok(_context.BusCategory.ToList());
        }
        [HttpDelete("DeleteImage/{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var response = await _photoService.DeleteImageAsync(id);

            if (response.StatusCode != 200)
                return StatusCode(response.StatusCode, response);

            return Ok(response);
        }

    }
}
