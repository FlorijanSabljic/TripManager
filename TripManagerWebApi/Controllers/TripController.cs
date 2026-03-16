using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripManagerWebApi.Dtos;
using TripManagerData.Models;
using Microsoft.AspNetCore.Authorization;

namespace TripManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private void Log(string message, string level = "Info")
        {
            _context.ApiLogs.Add(new ApiLog
            {
                Timestamp = DateTime.Now,
                Level = level,
                Message = message
            });
            _context.SaveChanges();
        }

        private readonly TripManagerDbContext _context;

        public TripController(TripManagerDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllTrips")]
        public ActionResult<IEnumerable<Trip>> GetAllTrips()
        {
            var trips = _context.Trips
                .Include(t => t.Destination)
                .Select(t => new TripDto
                {
                    Name = t.Name,
                    Description = t.Description,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    Price = t.Price,
                    MaxParticipants = t.MaxParticipants,
                    DestinationName = t.Destination.Name,
                    DestinationCountry = t.Destination.Country
                })
                .ToList();
            return Ok(trips);
        }

        [HttpGet("GetTripById/{id}")]
        public ActionResult<Trip> GetTripById(int id)
        {

            var trip = _context.Trips.Find(id);

            if (trip == null)
            {
                return NotFound();
            }

            var tripDto = new TripDto
            {
                Name = trip.Name,
                Description = trip.Description,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Price = trip.Price,
                MaxParticipants = trip.MaxParticipants,
                DestinationName = _context.Destinations.First(d => d.Id == trip.DestinationId).Name,
                DestinationCountry = _context.Destinations.First(d => d.Id == trip.DestinationId).Country
            };

            return Ok(tripDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateTrip")]
        public ActionResult<Trip> CreateTrip(TripDto tripDto)
        {

            var trips = _context.Trips;

            if (string.IsNullOrEmpty(tripDto.Name) || string.IsNullOrEmpty(tripDto.Description))
                return BadRequest("Name and Description are required.");

            // Check for duplicates
            if (trips.Any(t => t.Name == tripDto.Name))
                return Conflict("Trip with the same name already exists.");

            if (!_context.Destinations.Any(d => d.Name == tripDto.DestinationName))
                return BadRequest("Invalid Destination.");


            var trip = new Trip
            {
                Name = tripDto.Name,
                Description = tripDto.Description,
                StartDate = tripDto.StartDate,
                EndDate = tripDto.EndDate,
                Price = tripDto.Price,
                MaxParticipants = tripDto.MaxParticipants,
                DestinationId = _context.Destinations.First(d => d.Name == tripDto.DestinationName).Id,
                CreatedAt = DateTime.Now,
            };

            _context.Trips.Add(trip);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetTripById), new { id = trip.Id }, tripDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateTrip/{id}")]
        public ActionResult<Trip> UpdateTrip(int id, TripDto tripDto)
        {
            var trip = _context.Trips.Find(id);
            if (trip == null)
            {
                return NotFound();
            }

            trip.Name = tripDto.Name;
            trip.Description = tripDto.Description;
            trip.StartDate = tripDto.StartDate;
            trip.EndDate = tripDto.EndDate;
            trip.Price = tripDto.Price;
            trip.MaxParticipants = tripDto.MaxParticipants;
            if (!_context.Destinations.Any(d => d.Name == tripDto.DestinationName))
                return BadRequest("Invalid DestinationId.");
            trip.DestinationId = _context.Destinations.First(d => d.Name == tripDto.DestinationName).Id;

            _context.SaveChanges();

            return Ok(tripDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteTrip/{id}")]
        public ActionResult DeleteTrip(int id)
        {
            var trip = _context.Trips.Find(id);
            if (trip == null)
            {
                return NotFound();
            }
            _context.Trips.Remove(trip);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpGet("SearchTrips")]
        public ActionResult<IEnumerable<TripDto>> Search(string? name, int page = 1, int count = 10, string sort = "asc")
        {
            var query = _context.Trips.Include(t => t.Destination).AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(t => t.Name.Contains(name));

            var total = query.Count();

            if (sort.ToLower() == "desc")
                query = query.OrderByDescending(t => t.Name);
            else
                query = query.OrderBy(t => t.Name);

            var trips = query
                .Skip((page - 1) * count)
                .Take(count)
                .Select(t => new TripDto
                {
                    Name = t.Name,
                    Description = t.Description,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    Price = t.Price,
                    MaxParticipants = t.MaxParticipants,
                    DestinationName = t.Destination.Name,
                    DestinationCountry = t.Destination.Country
                })
                .ToList();

            Log($"Search trips by name='{name}', page={page}, count={count}. Returned {trips.Count} of {total} total.");

            return Ok(new
            {
                Total = total,
                Page = page,
                Count = count,
                Items = trips
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/api/logs/get/{n}")]
        public ActionResult<IEnumerable<ApiLog>> GetLogs(int n)
        {
            var logs = _context.ApiLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(n)
                .ToList();
            return Ok(logs);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/api/logs/count")]
        public ActionResult<int> GetLogCount()
        {
            return Ok(_context.ApiLogs.Count());
        }

    }
}
