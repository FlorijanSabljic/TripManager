using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TripManagerData.Models;
using TripManagerWebApi.Dtos;

namespace TripManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DestinationController : ControllerBase
    {
        private readonly TripManagerDbContext _context;

        public DestinationController(TripManagerDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllDestinations")]
        public ActionResult<IEnumerable<DestinationDto>> GetAllDestinations()
        {
            var destinations = _context.Destinations
                .Select(d => new DestinationDto
                {
                    Name = d.Name,
                    Country = d.Country,
                    Description = d.Description
                })
                .ToList();
            return Ok(destinations);
        }

        [HttpGet("GetDestinationById/{id}")]
        public ActionResult<DestinationDto> GetDestinationById(int id)
        {
            var destination = _context.Destinations.Find(id);
            if (destination == null)
            {
                return NotFound();
            }

            var destinationDto = new DestinationDto
            {
                Name = destination.Name,
                Country = destination.Country,
                Description = destination.Description
            };

            return Ok(destination);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateDestination")]
        public ActionResult<DestinationDto> CreateDestination(DestinationDto destinationDto)
        {
            var destination = new Destination
            {
                Name = destinationDto.Name,
                Country = destinationDto.Country,
                Description = destinationDto.Description
            };
            _context.Destinations.Add(destination);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetDestinationById), new { id = destination.Id }, destinationDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateDestination/{id}")]
        public ActionResult UpdateDestination(int id, DestinationDto destinationDto)
        {
            var destination = _context.Destinations.Find(id);
            if (destination == null)
            {
                return NotFound();
            }
            destination.Name = destinationDto.Name;
            destination.Country = destinationDto.Country;
            destination.Description = destinationDto.Description;
            _context.SaveChanges();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteDestination/{id}")]
        public ActionResult DeleteDestination(int id)
        {
            var destination = _context.Destinations.Find(id);
            if (destination == null)
            {
                return NotFound();
            }
            _context.Destinations.Remove(destination);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpGet("SearchDestinations")]
        public ActionResult<IEnumerable<DestinationDto>> SearchDestinations(string? name, int page = 1, int count = 10, string sort = "asc")
        {
            var query = _context.Destinations.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(d => d.Name.Contains(name));
            }

            query = sort.ToLower() == "desc"
                ? query.OrderByDescending(d => d.Name)
                : query.OrderBy(d => d.Name);

            var destinations = query
                .Skip((page - 1) * count)
                .Take(count)
                .Select(d => new DestinationDto
                {
                    Name = d.Name,
                    Country = d.Country,
                    Description = d.Description
                })
                .ToList();

            return Ok(destinations);
        }
    }
}