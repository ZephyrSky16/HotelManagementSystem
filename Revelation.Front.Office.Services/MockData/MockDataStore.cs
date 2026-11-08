using System;
using System.Collections.Generic;
using System.Linq;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Services.MockData
{
    /// <summary>
    /// In-memory data store that mirrors the original static site's
    /// js/data/*.js mock files 1:1, so the migrated Razor views render the
    /// exact same demo content. Reservation dates are generated relative to
    /// DateTime.Now (just like the original addDays() helpers), so the
    /// dashboard/arrivals/departures stay "current" whenever the app runs.
    /// Thread-safety is intentionally not a concern here: this is demo data
    /// for a single-process dev/demo site, standing in for the database
    /// until the EF6 repositories are wired in.
    /// </summary>
    public static partial class MockDataStore
    {
        public static List<RoomType> RoomTypes { get; private set; }
        public static List<Room> Rooms { get; private set; }
        public static List<Guest> Guests { get; private set; }
        public static List<Reservation> Reservations { get; private set; }
        public static List<RatePlan> RatePlans { get; private set; }
        public static List<ExtraCharge> ExtraCharges { get; private set; }
        public static List<MaintenanceRequest> MaintenanceRequests { get; private set; }
        public static List<HousekeepingLogEntry> HousekeepingLog { get; private set; }
        public static HotelSettings Settings { get; private set; }

        public static readonly List<string> MaintenancePersonnel = new List<string>
        {
            "Carlos Jimenez", "Derek Osei", "Priya Raman", "Tomas Novak", "Will Harrington"
        };

        public static readonly List<string> HousekeepingStaff = new List<string>
        {
            "Linda Park", "Teresa Gomez", "Angela Cruz", "Ben Tucker", "Rosa Fuentes"
        };

        private static int _nextId = 1;
        internal static int NextId() => _nextId++;

        internal static DateTime Today => DateTime.Today;
        internal static DateTime AddDays(int days) => DateTime.Today.AddDays(days);

        static MockDataStore()
        {
            Reseed();
        }

        /// <summary>Rebuilds the entire in-memory dataset from scratch (also used by unit tests / manual reset).</summary>
        public static void Reseed()
        {
            _nextId = 1;
            RoomTypes = SeedRoomTypes();
            Rooms = SeedRooms(RoomTypes);
            Guests = SeedGuests();
            Reservations = SeedReservations(Rooms, Guests);
            RatePlans = SeedRatePlans(RoomTypes);
            ExtraCharges = SeedExtraCharges(RoomTypes);
            MaintenanceRequests = SeedMaintenanceRequests(Rooms);
            HousekeepingLog = SeedHousekeepingLog();
            Settings = SeedSettings();
        }

        internal static RoomType RoomTypeByName(List<RoomType> types, string name) =>
            types.First(t => t.Name == name);

        internal static Room RoomByNumber(List<Room> rooms, string number) =>
            rooms.First(r => r.Number == number);

        internal static Guest GuestByIndex(List<Guest> guests, int oneBasedIndex) =>
            guests[oneBasedIndex - 1];
    }
}
