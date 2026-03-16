namespace TripManagerWebApi.Dtos
{
    public class TripBookingDto
    {
        public required string TripName { get; set; }
        public int NumberOfParticipants { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }


    }

}
