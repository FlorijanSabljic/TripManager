using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TripManagerData.Models;
using TripManagerWebApi.Dtos;

namespace TripManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripBookingController : ControllerBase
    {
        private readonly TripManagerDbContext _context;
        public TripBookingController(TripManagerDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("MyBookings")]
        public ActionResult<IEnumerable<TripBookingDto>> GetMyBookings()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
                return Unauthorized("User not authenticated.");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound("User not found.");

            var bookings = _context.TripBookings
                .Where(b => b.UserId == user.Id)
                .Select(b => new TripBookingDto
                {
                    TripName = b.Trip.Name,
                    NumberOfParticipants = b.NumberOfParticipants,
                    BookingDate = b.BookingDate
                })
                .ToList();

            return Ok(bookings);
        }

        [Authorize]
        [HttpPost("BookTrip")]
        public IActionResult BookTrip([FromBody] TripBookingDto bookingDto)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
                return Unauthorized("User not authenticated.");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound("User not found.");

            var trip = _context.Trips.FirstOrDefault(t => t.Name == bookingDto.TripName);
            if (trip == null)
                return NotFound("Trip not found.");

            var tripBooking = new TripBooking
            {
                UserId = user.Id,
                TripId = trip.Id,
                BookingDate = bookingDto.BookingDate,
                NumberOfParticipants = bookingDto.NumberOfParticipants,
                Status = "Booked"
            };
            _context.TripBookings.Add(tripBooking);
            _context.SaveChanges();

            return Ok("Trip booked successfully.");
        }

        [Authorize]
        [HttpPut("UpdateBooking/{tripName}")]
        public IActionResult UpdateBooking(string tripName, [FromBody] TripBookingDto dto)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
                return Unauthorized("User not authenticated.");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return Unauthorized("User not found.");

            var trip = _context.Trips.FirstOrDefault(t => t.Name == tripName);
            if (trip == null)
                return NotFound("Trip not found.");

            var booking = _context.TripBookings.FirstOrDefault(b => b.UserId == user.Id && b.TripId == trip.Id);
            if (booking == null)
                return NotFound("Booking not found.");

            booking.NumberOfParticipants = dto.NumberOfParticipants;
            booking.BookingDate = dto.BookingDate;

            _context.SaveChanges();

            return Ok("Booking updated.");
        }

        [Authorize]
        [HttpDelete("CancelBooking/{tripName}")]
        public IActionResult CancelBooking(string tripName)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
                return Unauthorized("User not authenticated.");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound("User not found.");

            var trip = _context.Trips.FirstOrDefault(t => t.Name == tripName);
            if (trip == null)
                return NotFound("Trip not found.");

            var booking = _context.TripBookings
                .FirstOrDefault(b => b.UserId == user.Id && b.TripId == trip.Id);

            if (booking == null)
                return NotFound("Booking not found.");

            _context.TripBookings.Remove(booking);
            _context.SaveChanges();
            return Ok("Booking cancelled successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("AllBookings")]
        public ActionResult<IEnumerable<TripBookingDto>> GetAllBookings()
        {
            var bookings = _context.TripBookings
                .Select(b => new TripBookingDto
                {
                    TripName = b.Trip.Name,
                    NumberOfParticipants = b.NumberOfParticipants,
                    BookingDate = b.BookingDate,
                    Status = b.Status
                })
                .ToList();

            return Ok(bookings);
        }
    }
}
