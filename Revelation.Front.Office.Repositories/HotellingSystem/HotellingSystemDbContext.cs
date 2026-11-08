using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.SqlServer;
using Revelation.Front.Office.Models.HotellingSystem;

namespace Revelation.Front.Office.Repositories.HotellingSystem
{
    /// <summary>
    /// Registers the SQL Server provider in code so the Web project needs no
    /// &lt;entityFramework&gt; config section. EF allows one DbConfiguration per
    /// assembly; this one also serves RevelationDbContext.
    /// </summary>
    public class RevelationDbConfiguration : DbConfiguration
    {
        public RevelationDbConfiguration()
        {
            SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
        }
    }

    /// <summary>
    /// Database-first context over the existing HotellingSystem database
    /// (connection string "HotellingSystem"). The initializer is disabled:
    /// EF must never create, drop or migrate this schema.
    /// </summary>
    [DbConfigurationType(typeof(RevelationDbConfiguration))]
    public class HotellingSystemDbContext : DbContext
    {
        static HotellingSystemDbContext()
        {
            Database.SetInitializer<HotellingSystemDbContext>(null);
        }

        public HotellingSystemDbContext() : base("name=HotellingSystem")
        {
        }

        public DbSet<Agent> Agents { get; set; }
        public DbSet<BookingCode> BookingCodes { get; set; }
        public DbSet<Cancellation> Cancellations { get; set; }
        public DbSet<ConfirmTo> ConfirmTos { get; set; }
        public DbSet<ConfirmVia> ConfirmVias { get; set; }
        public DbSet<Folio> Folios { get; set; }
        public DbSet<MarketCode> MarketCodes { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationStatus> ReservationStatuses { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Tables are singular (dbo.Agent, not dbo.Agents) and every text column is varchar.
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Properties<string>().Configure(p => p.IsUnicode(false));
            modelBuilder.Properties<System.DateTime>().Configure(p => p.HasColumnType("datetime"));

            MapMasterData(modelBuilder.Entity<Agent>(), "Agent");
            MapMasterData(modelBuilder.Entity<Folio>(), "Folio");
            MapMasterData(modelBuilder.Entity<BookingCode>(), "BookingCode");
            MapMasterData(modelBuilder.Entity<MarketCode>(), "MarketCode");
            MapMasterData(modelBuilder.Entity<Rate>(), "Rate");
            MapMasterData(modelBuilder.Entity<ConfirmTo>(), "ConfirmTo");
            MapMasterData(modelBuilder.Entity<ConfirmVia>(), "ConfirmVia");
            MapMasterData(modelBuilder.Entity<ReservationStatus>(), "ReservationStatus");
            MapMasterData(modelBuilder.Entity<Sale>(), "Sale");

            modelBuilder.Entity<Agent>().Property(e => e.FirstName).HasMaxLength(200);
            modelBuilder.Entity<Agent>().Property(e => e.LastName).HasMaxLength(200);
            modelBuilder.Entity<Agent>().Property(e => e.DisplayName).HasMaxLength(200);
            modelBuilder.Entity<Folio>().Property(e => e.FirstName).HasMaxLength(200);
            modelBuilder.Entity<Folio>().Property(e => e.LastName).HasMaxLength(200);
            modelBuilder.Entity<Folio>().Property(e => e.DisplayName).HasMaxLength(200);
            modelBuilder.Entity<BookingCode>().Property(e => e.Code).HasMaxLength(100);
            modelBuilder.Entity<BookingCode>().Property(e => e.Source).HasMaxLength(200);
            modelBuilder.Entity<MarketCode>().Property(e => e.Code).HasMaxLength(100);
            modelBuilder.Entity<Rate>().Property(e => e.Code).HasMaxLength(100);
            modelBuilder.Entity<ConfirmTo>().Property(e => e.Name).HasMaxLength(100);
            modelBuilder.Entity<ConfirmVia>().Property(e => e.Name).HasMaxLength(100);
            modelBuilder.Entity<ReservationStatus>().Property(e => e.Name).HasMaxLength(100);

            var cancellation = modelBuilder.Entity<Cancellation>();
            cancellation.ToTable("Cancellation");
            cancellation.Property(e => e.InsertedBy).HasMaxLength(200);

            var r = modelBuilder.Entity<Reservation>();
            r.ToTable("Reservation");
            r.Property(e => e.AgentName).HasMaxLength(200);
            r.Property(e => e.CardExpiry).HasMaxLength(50);
            r.Property(e => e.OTABookingReference).HasMaxLength(200);
            r.Property(e => e.CheckInRemarks).IsMaxLength();
            r.Property(e => e.CheckOutRemarks).IsMaxLength();
            r.Property(e => e.GroupCoordinator).IsMaxLength();
            r.Property(e => e.SettlementReference).IsMaxLength();

            // Relationships exist only in the model (the database declares no FKs).
            r.HasOptional(e => e.Agent).WithMany().HasForeignKey(e => e.AgentId);
            r.HasOptional(e => e.ConfirmTo).WithMany().HasForeignKey(e => e.ConfirmToId);
            r.HasOptional(e => e.ConfirmVia).WithMany().HasForeignKey(e => e.ConfirmViaId);
            r.HasOptional(e => e.LastCancellation).WithMany().HasForeignKey(e => e.LastCancellationId);
            r.HasOptional(e => e.MarketCode).WithMany().HasForeignKey(e => e.MarketCodeId);
            r.HasOptional(e => e.Rate).WithMany().HasForeignKey(e => e.RateId);
            r.HasOptional(e => e.ReservationStatus).WithMany().HasForeignKey(e => e.ReservationStatusId);
            r.HasOptional(e => e.Sales).WithMany().HasForeignKey(e => e.SalesId);
            r.HasOptional(e => e.BookingCode).WithMany().HasForeignKey(e => e.BookingCodeId);

            base.OnModelCreating(modelBuilder);
        }

        private static void MapMasterData<T>(EntityTypeConfiguration<T> config, string table) where T : MasterDataEntity
        {
            config.ToTable(table);
            config.Property(e => e.InsertedBy).HasMaxLength(200);
            config.Property(e => e.UpdatedBy).HasMaxLength(200);
        }
    }
}
