using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TripManagerData.Models;
using TripManagerWebApi.Dtos;

namespace TripManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {

        private readonly TripManagerDbContext _context;

        public ActivityController(TripManagerDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllActivities")]
        public ActionResult<IEnumerable<ActivityDto>> GetAllActivities()
        {
            var activitiesDto = _context.Activities.Select(a => new ActivityDto
            {
                Name = a.Name,
                Description = a.Description,
                DurationMinutes = a.Duration
            }).ToList();

            return Ok(activitiesDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateActivity")]

        public ActionResult<ActivityDto> CreateActivity([FromBody] ActivityDto activityDto)
        {
            if (_context.Activities
                .Any(a => a.Name == activityDto.Name && a.Duration == activityDto.DurationMinutes))
                return BadRequest("Activity with the same name and duration already exists.");

            var activity = new Activity
            {
                Name = activityDto.Name,
                Description = activityDto.Description,
                Duration = activityDto.DurationMinutes
            };
            _context.Activities.Add(activity);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetAllActivities), new { id = activity.Id }, activityDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateActivity/{id}")]
        public ActionResult<ActivityDto> UpdateActivity(int id, [FromBody] ActivityDto activityDto)
        {
            var activity = _context.Activities.Find(id);
            if (activity == null)
                return NotFound("Activity not found.");

            activity.Name = activityDto.Name;
            activity.Description = activityDto.Description;
            activity.Duration = activityDto.DurationMinutes;

            _context.SaveChanges();
            return Ok(activityDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteActivity/{id}")]
        public IActionResult DeleteActivity(int id)
        {
            var activity = _context.Activities.Find(id);
            if (activity == null)
                return NotFound("Activity not found.");
            _context.Activities.Remove(activity);
            _context.SaveChanges();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteActivity")]
        public IActionResult DeleteActivityById(int id)
        {
            var activity = _context.Activities.Find(id);
            if (activity == null)
                return NotFound("Activity not found.");
            _context.Activities.Remove(activity);
            _context.SaveChanges();
            return NoContent();
        }

    }
}
