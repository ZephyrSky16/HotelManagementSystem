using System.Data.Entity;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Repositories.EF
{
    /// <summary>
    /// Code-first EF6 context for the Revelation database. Not yet wired into
    /// the Web project's DI/composition root -- Controllers currently run
    /// against the in-memory mock data in Revelation.Front.Office.Services so
    /// the migrated site works without requiring a configured SQL instance.
    /// This context is ready to be swapped in: point the "RevelationDb"
    /// connection string at a real (or LocalDB) database and register the
    /// EF-backed repository implementations instead of the mock services.
    /// </summary>
    public class RevelationDbContext : DbContext
    {
        public RevelationDbContext() : base("name=RevelationDb")
        {
        }

        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<RatePlan> RatePlans { get; set; }
        public DbSet<ExtraCharge> ExtraCharges { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<HousekeepingLogEntry> HousekeepingLogEntries { get; set; }
        public DbSet<HotelSettings> HotelSettings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>()
                .HasRequired(r => r.RoomType)
                .WithMany(t => t.Rooms)
                .HasForeignKey(r => r.RoomTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Reservation>()
                .HasRequired(r => r.Guest)
                .WithMany(g => g.Reservations)
                .HasForeignKey(r => r.GuestId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Reservation>()
                .HasRequired(r => r.RoomType)
                .WithMany()
                .HasForeignKey(r => r.RoomTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Reservation>()
                .HasOptional(r => r.Room)
                .WithMany(room => room.Reservations)
                .HasForeignKey(r => r.RoomId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RatePlan>()
                .HasRequired(p => p.RoomType)
                .WithMany(t => t.RatePlans)
                .HasForeignKey(p => p.RoomTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ExtraCharge>()
                .HasRequired(e => e.RoomType)
                .WithOptional(t => t.ExtraCharge);

            modelBuilder.Entity<MaintenanceRequest>()
                .HasRequired(m => m.Room)
                .WithMany(room => room.MaintenanceRequests)
                .HasForeignKey(m => m.RoomId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HousekeepingLogEntry>()
                .HasRequired(h => h.Room)
                .WithMany(room => room.HousekeepingLogEntries)
                .HasForeignKey(h => h.RoomId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Room>()
                .Property(r => r.Number).HasMaxLength(10).IsRequired();
            modelBuilder.Entity<Reservation>()
                .Property(r => r.ReservationNo).HasMaxLength(20).IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
