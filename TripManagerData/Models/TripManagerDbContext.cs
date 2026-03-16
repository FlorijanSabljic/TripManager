using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TripManagerData.Models;

public partial class TripManagerDbContext : DbContext
{
    public TripManagerDbContext()
    {
    }

    public TripManagerDbContext(DbContextOptions<TripManagerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<ApiLog> ApiLogs { get; set; }

    public virtual DbSet<Destination> Destinations { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    public virtual DbSet<TripBooking> TripBookings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=ConnectionStrings:TripManagerConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Activity__3214EC07BE4D1E59");
        });

        modelBuilder.Entity<ApiLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ApiLog__3214EC0768CEDF61");
        });

        modelBuilder.Entity<Destination>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Destinat__3214EC07C875E1B5");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Image__3214EC079BCE315F");

            entity.HasOne(d => d.Trip).WithMany(p => p.Images).HasConstraintName("FK_Image_Trip");
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Trip__3214EC0743A7A3CC");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Destination).WithMany(p => p.Trips).HasConstraintName("FK_Trip_Destination");

            entity.HasMany(d => d.Activities).WithMany(p => p.Trips)
                .UsingEntity<Dictionary<string, object>>(
                    "TripActivity",
                    r => r.HasOne<Activity>().WithMany()
                        .HasForeignKey("ActivityId")
                        .HasConstraintName("FK_TripActivity_Activity"),
                    l => l.HasOne<Trip>().WithMany()
                        .HasForeignKey("TripId")
                        .HasConstraintName("FK_TripActivity_Trip"),
                    j =>
                    {
                        j.HasKey("TripId", "ActivityId").HasName("PK__TripActi__55833B470341AAB4");
                        j.ToTable("TripActivity");
                    });
        });

        modelBuilder.Entity<TripBooking>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.TripId }).HasName("PK__TripBook__E2950B5F28A9F35A");

            entity.Property(e => e.BookingDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NumberOfParticipants).HasDefaultValue(1);
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.Trip).WithMany(p => p.TripBookings).HasConstraintName("FK_TripBooking_Trip");

            entity.HasOne(d => d.User).WithMany(p => p.TripBookings).HasConstraintName("FK_TripBooking_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07AD40C3DB");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Role).HasDefaultValue("User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
