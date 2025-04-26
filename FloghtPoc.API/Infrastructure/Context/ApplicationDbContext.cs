using FlightPoc.API.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlightPoc.API.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FlightEntity> Flights { get; set; }
        public DbSet<PassangerEntity> Passangers { get; set; }
        public DbSet<BaggageEntity> BaggageItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FlightEntity>()
                .ToTable("Flights")
                .Property(f => f.RowVersion)
                .IsRowVersion()
                .IsRequired();

            modelBuilder.Entity<BaggageEntity>()
                .ToTable("BaggageItems");


            modelBuilder.Entity<PassangerEntity>()
                .ToTable("Passangers")
                .HasIndex(p => new { p.PassangerUniqueId, p.FlightId })
                .IsUnique();

            modelBuilder.Entity<FlightEntity>().HasData(
                new FlightEntity
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    FlightNumber = "QF123",
                    MaxBaggagePerPassanger = 50.0,
                    SeatCapacity = 100,
                    TotalPassangers = 0,
                    RowVersion = new byte[] { 0, 0, 0, 0 }
                }
            );
        }
    }
}
