using System.ComponentModel;

namespace TripManagerWebApp.Models.ViewModels
{
    public class TripVM
    {
        public int? Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal Price { get; set; }
        [DisplayName("Max Participants")]
        public int MaxParticipants { get; set; }
        [DisplayName("Destination")]
        public string? DestinationName { get; set; }
        [DisplayName("Country")]
        public string? DestinationCountry { get; set; }
    }
}
