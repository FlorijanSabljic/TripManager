using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TripManagerData.Models;

[Table("ApiLog")]
public partial class ApiLog
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Timestamp { get; set; }

    [StringLength(20)]
    public string Level { get; set; } = null!;

    [StringLength(500)]
    public string Message { get; set; } = null!;
}
