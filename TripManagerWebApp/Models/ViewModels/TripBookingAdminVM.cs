namespace TripManagerWebApp.Models.ViewModels
{
    public class TripBookingAdminVM
    {
        public required string TripName { get; set; }
        public int NumberOfParticipants { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public string Username { get; set; }
    }
}
