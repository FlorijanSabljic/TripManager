using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TripManagerData.Models;

[Table("Activity")]
[Index("Name", Name = "UQ__Activity__737584F60D45BAF4", IsUnique = true)]
public partial class Activity
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    public int Duration { get; set; }

    [ForeignKey("ActivityId")]
    [InverseProperty("Activities")]
    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
