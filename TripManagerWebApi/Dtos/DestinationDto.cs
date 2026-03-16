namespace TripManagerWebApi.Dtos
{
    public class DestinationDto
    {
        public required string Name { get; set; }
        public required string Country { get; set; }
        public string? Description { get; set; }
    }
}
