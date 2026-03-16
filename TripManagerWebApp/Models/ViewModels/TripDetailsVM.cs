using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TripManagerWebApp.Models.ViewModels
{
    public class TripDetailsVM
    {
        public int? Id { get; set; }

        [DisplayName("Trip Name")]
        public required string Name { get; set; }

        [DisplayName("Description")]
        public required string Description { get; set; }

        [DisplayName("Start Date")]
        public DateOnly StartDate { get; set; }

        [DisplayName("End Date")]
        public DateOnly EndDate { get; set; }

        [DisplayName("Price")]
        public decimal Price { get; set; }

        [DisplayName("Max Participants")]
        public int MaxParticipants { get; set; }

        [DisplayName("Destination")]
        public string? DestinationName { get; set; }

        [DisplayName("Country")]
        public string? DestinationCountry { get; set; }

        [DisplayName("Activities")]
        public List<ActivityVM> Activities { get; set; } = new();
    }
}
