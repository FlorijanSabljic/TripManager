using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TripManagerData.Models;

[Table("Image")]
public partial class Image
{
    [Key]
    public int Id { get; set; }

    public int TripId { get; set; }

    [StringLength(500)]
    public string ImageUrl { get; set; } = null!;

    [StringLength(200)]
    public string? AltText { get; set; }

    [ForeignKey("TripId")]
    [InverseProperty("Images")]
    public virtual Trip Trip { get; set; } = null!;
}
