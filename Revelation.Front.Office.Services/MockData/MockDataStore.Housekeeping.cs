using System.Collections.Generic;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Services.MockData
{
    public static partial class MockDataStore
    {
        private static List<HousekeepingLogEntry> SeedHousekeepingLog()
        {
            HousekeepingLogEntry H(string roomNo, string staff, int dayOffset, HousekeepingStatus from, HousekeepingStatus to, int minutes) =>
                new HousekeepingLogEntry
                {
                    Id = NextId(), RoomId = RoomByNumber(Rooms, roomNo).Id, Room = RoomByNumber(Rooms, roomNo),
                    Staff = staff, EntryDate = AddDays(dayOffset), FromStatus = from, ToStatus = to, DurationMinutes = minutes
                };

            return new List<HousekeepingLogEntry>
            {
                H("107", "Linda Park", -1, HousekeepingStatus.Dirty, HousekeepingStatus.Inspected, 35),
                H("201", "Teresa Gomez", -1, HousekeepingStatus.Dirty, HousekeepingStatus.Clean, 28),
                H("310", "Angela Cruz", -2, HousekeepingStatus.Dirty, HousekeepingStatus.Inspected, 40),
                H("404", "Rosa Fuentes", -2, HousekeepingStatus.Dirty, HousekeepingStatus.Clean, 31),
                H("109", "Ben Tucker", -3, HousekeepingStatus.Dirty, HousekeepingStatus.Inspected, 45),
                H("302", "Linda Park", -3, HousekeepingStatus.Dirty, HousekeepingStatus.Clean, 29),
                H("401", "Teresa Gomez", -4, HousekeepingStatus.Dirty, HousekeepingStatus.Inspected, 33),
                H("203", "Angela Cruz", -4, HousekeepingStatus.Dirty, HousekeepingStatus.Clean, 27)
            };
        }

        private static HotelSettings SeedSettings()
        {
            return new HotelSettings
            {
                Id = NextId(),
                HotelName = "Harborview Grand Hotel & Suites",
                LegalName = "Harborview Grand Hospitality LLC",
                Address = "1 Harborview Plaza, Bayfront District, San Francisco, CA 94111",
                Phone = "+1 (415) 555-0100",
                Email = "frontdesk@harborviewgrand.com",
                Website = "www.harborviewgrand.com",
                CheckInTime = "15:00",
                CheckOutTime = "11:00",
                Currency = "USD",
                Timezone = "America/Los_Angeles",
                TaxRate = 12,
                ServiceCharge = 5
            };
        }
    }
}
