using System;
using System.Data.Entity;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Repositories.EF
{
    /// <summary>
    /// Code-first database initializer with a minimal seed, demonstrating the
    /// intended seeding pattern for whoever wires this context up later.
    /// Not registered/active until the project actually switches the Web
    /// layer over to the EF-backed repositories.
    /// </summary>
    public class RevelationDbInitializer : DropCreateDatabaseIfModelChanges<RevelationDbContext>
    {
        protected override void Seed(RevelationDbContext context)
        {
            var standard = new RoomType
            {
                Name = "Standard Room",
                Description = "A comfortable, well-appointed room ideal for solo travelers and couples.",
                MaxOccupancy = 2,
                MaxAdults = 2,
                MaxChildren = 1,
                BedConfiguration = "1 Queen Bed",
                BaseRate = 89.00m,
                AmenitiesCsv = "Free WiFi,32\" TV,Air Conditioning,Private Bathroom",
                Active = true
            };
            context.RoomTypes.Add(standard);

            context.Rooms.Add(new Room
            {
                Number = "101",
                Floor = 1,
                Capacity = 2,
                BedConfiguration = "1 Queen Bed",
                BaseRate = 89.00m,
                Status = RoomStatus.Available,
                HousekeepingStatus = HousekeepingStatus.Clean,
                Active = true,
                RoomType = standard
            });

            context.HotelSettings.Add(new HotelSettings
            {
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
            });

            base.Seed(context);
        }
    }
}
