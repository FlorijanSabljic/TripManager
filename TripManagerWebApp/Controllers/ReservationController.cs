using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TripManagerData.Models;
using TripManagerWebApp.Models.ViewModels;

namespace TripManagerWebApp.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly TripManagerDbContext _context;

        public ReservationController(TripManagerDbContext context)
        {
            _context = context;
        }

        #region Trips for Reservation listing
        public IActionResult TripsForReservation()
        {
            try
            {
                var trips = _context.Trips.Include(t => t.Destination).ToList();

                var tripsVm = trips.Select(t => new TripVM
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Price = t.Price,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    MaxParticipants = t.MaxParticipants,
                    DestinationName = t.Destination?.Name,
                    DestinationCountry = t.Destination?.Country
                }).ToList();

                return View(tripsVm);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred while retrieving trips. Please try again later.");
                return View(new List<TripVM>());
            }
        }

        #endregion

        #region Book Trip
        public IActionResult BookTrip(int id)
        {
            try
            {
                var trip = _context.Trips.Include(t => t.Destination).FirstOrDefault(t => t.Id == id);

                if (trip == null)
                {
                    return NotFound();
                }

                var tripDetailsVm = new TripDetailsVM
                {
                    Id = trip.Id,
                    Name = trip.Name,
                    Description = trip.Description,
                    Price = trip.Price,
                    StartDate = trip.StartDate,
                    EndDate = trip.EndDate,
                    MaxParticipants = trip.MaxParticipants,
                    DestinationName = trip.Destination?.Name,
                    DestinationCountry = trip.Destination?.Country
                };

                return View(tripDetailsVm);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred while retrieving trip details. Please try again later.");
                return View();
            }
        }

        [HttpPost]
        public IActionResult BookTrip(TripDetailsVM tripDetailsVm)
        {
            try
            {
                int id = tripDetailsVm.Id ?? 0;

                var trip = _context.Trips
                    .Include(t => t.Destination)
                    .FirstOrDefault(t => t.Id == id);
                if (trip == null)
                {
                    return NotFound();
                }

                var username = User.FindFirstValue(ClaimTypes.Name);
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    return Unauthorized();
                }

                var existingBooking = _context.TripBookings
                    .FirstOrDefault(tb => tb.UserId == user.Id && tb.TripId == trip.Id);
                if (existingBooking != null)
                {
                    ModelState.AddModelError("", "You have already booked this trip.");
                    tripDetailsVm = new TripDetailsVM
                    {
                        Id = trip.Id,
                        Name = trip.Name,
                        Description = trip.Description,
                        Price = trip.Price,
                        StartDate = trip.StartDate,
                        EndDate = trip.EndDate,
                        MaxParticipants = trip.MaxParticipants,
                        DestinationName = trip.Destination?.Name,
                        DestinationCountry = trip.Destination?.Country
                    };
                    return View(tripDetailsVm);
                }

                var tripBooking = new TripBooking
                {
                    UserId = user.Id,
                    TripId = trip.Id,
                    BookingDate = DateTime.Now,
                    NumberOfParticipants = 1,
                    Status = "Booked"
                };

                _context.TripBookings.Add(tripBooking);
                _context.SaveChanges();

                return RedirectToAction("MyBookings");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred while booking the trip: " + ex.Message);
                return View(tripDetailsVm);
            }
        }

        #endregion

        #region View Bookings (Admin/User)
        public IActionResult MyBookings()
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.Name);
                if (string.IsNullOrEmpty(username))
                {
                    ModelState.AddModelError("", "User not authenticated.");
                    return View();
                }


                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return View();
                }

                var bookingsVm = _context.TripBookings
                    .Where(b => b.UserId == user.Id)
                    .Select(b => new TripBookingVM
                    {
                        TripName = b.Trip.Name,
                        NumberOfParticipants = b.NumberOfParticipants,
                        BookingDate = b.BookingDate
                    })
                    .ToList();

                return View(bookingsVm);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred while retrieving your bookings. Please try again later.");
                return View(new List<TripBookingVM>());
            }
        }

        public IActionResult AllBookings()
        {
            try
            {
                var bookings = _context.TripBookings
                        .Include(b => b.User)
                        .Include(b => b.Trip)
                        .ToList();

                var bookingsVm = bookings.Select(b => new TripBookingAdminVM
                {
                    TripName = b.Trip.Name,
                    NumberOfParticipants = b.NumberOfParticipants,
                    BookingDate = b.BookingDate,
                    Status = b.Status,
                    Username = b.User.Username
                }).ToList();

                return View(bookingsVm);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred while retrieving all bookings. Please try again later.");
                return View(new List<TripBookingAdminVM>());
            }
        }

        #endregion

        #region Booking Details and Cancellation

        public IActionResult BookingDetails(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return NotFound();
                }

                var username = User.FindFirstValue(ClaimTypes.Name);
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    return Unauthorized();
                }

                var booking = new TripBooking();

                if (user.Role == "User")
                {
                    booking = _context.TripBookings
                            .Include(b => b.Trip)
                            .FirstOrDefault(b => b.UserId == user.Id && b.Trip.Name == name);
                }
                else
                {
                    booking = _context.TripBookings
                            .Include(b => b.Trip)
                            .Include(b => b.User)
                            .FirstOrDefault(b => b.Trip.Name == name);
                }

                if (booking == null)
                {
                    return NotFound();
                }

                var bookingDetailsVm = new TripBookingVM
                {
                    TripName = booking.Trip.Name,
                    NumberOfParticipants = booking.NumberOfParticipants,
                    BookingDate = booking.BookingDate
                };

                return View(bookingDetailsVm);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred while retrieving booking details. Please try again later.");
                return View();
            }
        }

        [HttpPost]
        public IActionResult CancelBooking(string tripName)
        {
            try
            {
                var tripBooking = _context.TripBookings
            .Include(b => b.Trip)
            .FirstOrDefault(b => b.Trip.Name == tripName);

                if (tripBooking == null)
                {
                    return NotFound();
                }
                _context.TripBookings.Remove(tripBooking);
                _context.SaveChanges();
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("AllBookings");
                }
                return RedirectToAction("MyBookings");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred while cancelling the booking. Please try again later.");
                return RedirectToAction("BookingDetails", new { name = tripName });
            }
        }

        #endregion
    }
}
