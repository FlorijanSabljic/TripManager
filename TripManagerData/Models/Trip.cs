using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TripManagerData.Models;

[Table("Trip")]
[Index("DestinationId", Name = "IX_Trip_DestinationId")]
[Index("StartDate", Name = "IX_Trip_StartDate")]
[Index("Name", Name = "UQ__Trip__737584F61CE7C0EA", IsUnique = true)]
public partial class Trip
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    public string Name { get; set; } = null!;

    [StringLength(1000)]
    public string Description { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    public int MaxParticipants { get; set; }

    public int DestinationId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("DestinationId")]
    [InverseProperty("Trips")]
    public virtual Destination Destination { get; set; } = null!;

    [InverseProperty("Trip")]
    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    [InverseProperty("Trip")]
    public virtual ICollection<TripBooking> TripBookings { get; set; } = new List<TripBooking>();

    [ForeignKey("TripId")]
    [InverseProperty("Trips")]
    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
