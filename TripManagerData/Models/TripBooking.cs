using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TripManagerData.Models;

[PrimaryKey("UserId", "TripId")]
[Table("TripBooking")]
[Index("TripId", Name = "IX_TripBooking_TripId")]
[Index("UserId", Name = "IX_TripBooking_UserId")]
public partial class TripBooking
{
    [Key]
    public int UserId { get; set; }

    [Key]
    public int TripId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime BookingDate { get; set; }

    public int NumberOfParticipants { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    [ForeignKey("TripId")]
    [InverseProperty("TripBookings")]
    public virtual Trip Trip { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("TripBookings")]
    public virtual User User { get; set; } = null!;
}
