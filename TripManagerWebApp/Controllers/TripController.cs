using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using NuGet.Protocol;
using TripManagerData.Models;
using TripManagerWebApp.Models;
using TripManagerWebApp.Models.ViewModels;

namespace TripManagerWebApp.Controllers
{
    [Authorize]
    public class TripController : Controller
    {
        private readonly TripManagerDbContext _context;

        private readonly IConfiguration _configuration;

        public TripController(TripManagerDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        #region All Trips
        public IActionResult AllTrips()
        {
            var tripsVm = _context.Trips
                .Include(t => t.Destination)
                .ToList()
                .Select(t => new TripVM
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    Price = t.Price,
                    MaxParticipants = t.MaxParticipants,
                    DestinationName = t.Destination?.Name,
                    DestinationCountry = t.Destination?.Country
                });

            return View("AllTrips", tripsVm);
        }

        #endregion

        #region Create
        public IActionResult Create()
        {
            var countries = _context.Destinations
                .Select(d => d.Country)
                .Distinct()
                .ToList();

            ViewBag.Countries = countries;
            return View();
        }

        [HttpPost]
        public IActionResult Create(TripVM tripVm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var countries = _context.Destinations
                        .Select(d => d.Country)
                        .Distinct()
                        .ToList();
                    ViewBag.Countries = countries;


                    foreach (var key in ModelState.Keys)
                    {
                        var errors = ModelState[key].Errors;
                        foreach (var error in errors)
                        {
                            Console.WriteLine($"{key}: {error.ErrorMessage}");
                        }
                    }
                    return View(tripVm);
                }
                var destination = _context.Destinations
                    .FirstOrDefault(d => d.Name == tripVm.DestinationName && d.Country == tripVm.DestinationCountry);
                if (destination == null)
                {
                    ModelState.AddModelError("", "Invalid destination.");
                    var countries = _context.Destinations
                        .Select(d => d.Country)
                        .Distinct()
                        .ToList();
                    ViewBag.Countries = countries;
                    return View(tripVm);
                }
                var trip = new Trip
                {
                    Name = tripVm.Name,
                    Description = tripVm.Description,
                    StartDate = tripVm.StartDate,
                    EndDate = tripVm.EndDate,
                    Price = tripVm.Price,
                    MaxParticipants = tripVm.MaxParticipants,
                    DestinationId = destination.Id
                };
                _context.Trips.Add(trip);
                _context.SaveChanges();
                return RedirectToAction("AllTrips");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred while creating the trip.");
                var countries = _context.Destinations
                    .Select(d => d.Country)
                    .Distinct()
                    .ToList();
                ViewBag.Countries = countries;
                return View(tripVm);
            }
        }

        #endregion

        #region Edit
        public IActionResult Edit(int id)
        {
            var trip = _context.Trips.Find(id);
            if (trip == null)
            {
                return NotFound();
            }

            var tripVm = new TripVM
            {
                Id = trip.Id,
                Name = trip.Name,
                Description = trip.Description,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Price = trip.Price,
                MaxParticipants = trip.MaxParticipants,
                DestinationName = trip.Destination?.Name,
                DestinationCountry = trip.Destination?.Country
            };

            return View(tripVm);
        }

        [HttpPost]
        public IActionResult Edit(TripVM tripVm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(tripVm);
                }
                var trip = _context.Trips.Find(tripVm.Id);
                if (trip == null)
                {
                    return NotFound();
                }
                trip.Name = tripVm.Name;
                trip.Description = tripVm.Description;
                trip.StartDate = tripVm.StartDate;
                trip.EndDate = tripVm.EndDate;
                trip.Price = tripVm.Price;
                trip.MaxParticipants = tripVm.MaxParticipants;
                _context.SaveChanges();
                return RedirectToAction("AllTrips");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred while editing the trip.");
                return View(tripVm);
            }
        }

        #endregion

        #region Delete
        public IActionResult Delete(int id)
        {
            var trip = _context.Trips.Find(id);
            if (trip == null)
            {
                return NotFound();
            }
            var tripVm = new TripVM
            {
                Id = trip.Id,
                Name = trip.Name,
                Description = trip.Description,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Price = trip.Price,
                MaxParticipants = trip.MaxParticipants,
                DestinationName = trip.Destination?.Name,
                DestinationCountry = trip.Destination?.Country
            };

            return View(tripVm);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var trip = _context.Trips.Find(id);
                if (trip == null)
                {
                    return NotFound();
                }
                _context.Trips.Remove(trip);
                _context.SaveChanges();
                return RedirectToAction("AllTrips");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred while deleting the trip.");
                return RedirectToAction("Delete", new { id = id });
            }
        }

        #endregion

        #region Details
        public IActionResult Details(int id)
        {
            var trip = _context.Trips
                .Include(t => t.Destination)
                .FirstOrDefault(t => t.Id == id);
            if (trip == null)
            {
                return NotFound();
            }

            var tripDetailsVm = new TripDetailsVM
            {
                Id = trip.Id,
                Name = trip.Name,
                Description = trip.Description,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Price = trip.Price,
                MaxParticipants = trip.MaxParticipants,
                DestinationName = trip.Destination?.Name,
                DestinationCountry = trip.Destination?.Country,
                Activities = trip.Activities.Select(a => new ActivityVM
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Duration = a.Duration
                }).ToList()
            };
            return View(tripDetailsVm);
        }

        #endregion

        #region SearchTrips
        public IActionResult SearchTrips(SearchVM searchVm)
        {
            var trips = _context.Trips.Include(t => t.Destination).AsQueryable();

            // Filter by name
            if (string.IsNullOrEmpty(searchVm.Query) && string.IsNullOrEmpty(searchVm.Submit))
            {
                searchVm.Query = Request.Cookies["query"] ?? "";
            }
            else
            {
                // save last search for 10 minutes
                Response.Cookies.Append("query", searchVm.Query ?? "", new()
                {
                    Expires = DateTime.Now.AddMinutes(5)
                });
            }
            trips = trips.Where(t => t.Name.Contains(searchVm.Query ?? ""));


            // Sort (name, price, destination, participants)
            if (searchVm.OrderBy is not null)
            {
                switch (searchVm.OrderBy)
                {
                    case "name_asc":
                        trips = trips.OrderBy(t => t.Name);
                        break;
                    case "name_desc":
                        trips = trips.OrderByDescending(t => t.Name);
                        break;
                    case "price_asc":
                        trips = trips.OrderBy(t => t.Price);
                        break;
                    case "price_desc":
                        trips = trips.OrderByDescending(t => t.Price);
                        break;
                    case "destination_asc":
                        trips = trips.OrderBy(t => t.Destination.Name);
                        break;
                    case "destination_desc":
                        trips = trips.OrderByDescending(t => t.Destination.Name);
                        break;
                    case "participants_asc":
                        trips = trips.OrderBy(t => t.MaxParticipants);
                        break;
                    case "participants_desc":
                        trips = trips.OrderByDescending(t => t.MaxParticipants);
                        break;

                    default:
                        trips = trips.OrderBy(t => t.Id);
                        break;
                }
            }

            var filteredCount = trips.Count();
            // begin pager
            var extendPages = _configuration.GetValue<int>("Paging:ExtendPages");
            searchVm.LastPage = (int)Math.Ceiling(1.0 * filteredCount / searchVm.PageSize);
            searchVm.FromPager = searchVm.Page > extendPages ?
                searchVm.Page - extendPages : 1;
            searchVm.ToPager = searchVm.Page + extendPages < searchVm.LastPage ?
                searchVm.Page + extendPages : searchVm.LastPage;
            // end pager


            // Pagination

            if (searchVm.Page == 0)
            {
                searchVm.Page = 1;
            }

            if (searchVm.PageSize == 0)
            {
                searchVm.PageSize = trips.Count();
            }

            trips = trips
            .Skip((searchVm.Page - 1) * searchVm.PageSize)
            .Take(searchVm.PageSize);

            var tripsVm = trips.Select(t => new TripVM
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Price = t.Price,
                MaxParticipants = t.MaxParticipants,
                DestinationName = t.Destination.Name,
                DestinationCountry = t.Destination.Country
            });

            searchVm.Trips = tripsVm.ToList();

            ViewData["CurrentCookieQuery"] = searchVm.Query;
            return View(searchVm);
        }

        #endregion

        #region Select Destination Cities AJAX

        [HttpGet]
        public JsonResult GetCities(string country)
        {
            var cities = _context.Destinations
                .Where(d => d.Country == country)
                .Select(d => d.Name)
                .Distinct()
                .ToList();
            return Json(cities);
        }

        #endregion
    }
}
