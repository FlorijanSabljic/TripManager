using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TripManagerData.Models;

[Table("Destination")]
[Index("Name", Name = "UQ__Destinat__737584F6B6A4A151", IsUnique = true)]
public partial class Destination
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string Country { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [InverseProperty("Destination")]
    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
