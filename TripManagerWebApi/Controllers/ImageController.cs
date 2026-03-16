using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TripManagerData.Models;

namespace TripManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly TripManagerDbContext _context;
        public ImageController(TripManagerDbContext context)
        {
            _context = context;
        }
    }
}
