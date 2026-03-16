namespace TripManagerWebApi.Dtos
{
    public class ActivityDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required int DurationMinutes { get; set; }
    }
}
