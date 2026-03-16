namespace TripManagerWebApi.Dtos
{
    public class TripDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal Price { get; set; }
        public int MaxParticipants { get; set; }
        public string? DestinationName { get; set; }
        public string? DestinationCountry { get; set; }
    }
}
