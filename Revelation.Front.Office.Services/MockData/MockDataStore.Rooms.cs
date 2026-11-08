using System.Collections.Generic;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Services.MockData
{
    public static partial class MockDataStore
    {
        private static List<Room> SeedRooms(List<RoomType> types)
        {
            var std = RoomTypeByName(types, "Standard Room");
            var dlx = RoomTypeByName(types, "Deluxe Room");
            var sup = RoomTypeByName(types, "Superior Room");
            var fam = RoomTypeByName(types, "Family Room");
            var ste = RoomTypeByName(types, "Suite");
            var exe = RoomTypeByName(types, "Executive Suite");

            Room R(string number, int floor, RoomType type, int cap, string bed, decimal rate, RoomStatus status, HousekeepingStatus hk) =>
                new Room
                {
                    Id = NextId(), Number = number, Floor = floor, RoomType = type, RoomTypeId = type.Id,
                    Capacity = cap, BedConfiguration = bed, BaseRate = rate, Status = status,
                    HousekeepingStatus = hk, Active = status != RoomStatus.OutOfOrder
                };

            return new List<Room>
            {
                // Floor 1 -- Standard (101-108), Family (109-110)
                R("101", 1, std, 2, "1 Queen Bed", 89, RoomStatus.Available, HousekeepingStatus.Clean),
                R("102", 1, std, 2, "1 Queen Bed", 89, RoomStatus.Occupied, HousekeepingStatus.Occupied),
                R("103", 1, std, 2, "1 Queen Bed", 89, RoomStatus.Occupied, HousekeepingStatus.Occupied),
                R("104", 1, std, 2, "1 Queen Bed", 89, RoomStatus.Available, HousekeepingStatus.Clean),
                R("105", 1, std, 2, "1 Queen Bed", 89, RoomStatus.Reserved, HousekeepingStatus.Clean),
                R("106", 1, std, 2, "1 Queen Bed", 89, RoomStatus.Available, HousekeepingStatus.Dirty),
                R("107", 1, std, 2, "1 Queen Bed", 89, RoomStatus.Occupied, HousekeepingStatus.Occupied),
                R("108", 1, std, 2, "1 Queen Bed", 89, RoomStatus.UnderMaintenance, HousekeepingStatus.Dirty),
                R("109", 1, fam, 5, "2 Queen Beds + Sofa Bed", 159, RoomStatus.Available, HousekeepingStatus.Inspected),
                R("110", 1, fam, 5, "2 Queen Beds + Sofa Bed", 159, RoomStatus.Occupied, HousekeepingStatus.Occupied),

                // Floor 2 -- Deluxe (201-208), Family (209-210)
                R("201", 2, dlx, 3, "1 King Bed", 119, RoomStatus.Occupied, HousekeepingStatus.Occupied),
                R("202", 2, dlx, 3, "1 King Bed", 119, RoomStatus.Available, HousekeepingStatus.Clean),
                R("203", 2, dlx, 3, "1 King Bed", 119, RoomStatus.Reserved, HousekeepingStatus.Clean),
                R("204", 2, dlx, 3, "1 King Bed", 119, RoomStatus.Occupied, HousekeepingStatus.Occupied),
                R("205", 2, dlx, 3, "1 King Bed", 119, RoomStatus.Available, HousekeepingStatus.Dirty),
                R("206", 2, dlx, 3, "1 King Bed", 119, RoomStatus.Available, HousekeepingStatus.Inspected),
                R("207", 2, dlx, 3, "1 King Bed", 119, RoomStatus.Occupied, HousekeepingStatus.Occupied),
                R("208", 2, dlx, 3, "1 King Bed", 119, RoomStatus.OutOfOrder, HousekeepingStatus.Dirty),
                R("209", 2, fam, 5, "2 Queen Beds + Sofa Bed", 159, RoomStatus.Available, HousekeepingStatus.Clean),
                R("210", 2, fam, 5, "2 Queen Beds + Sofa Bed", 159, RoomStatus.Reserved, HousekeepingStatus.Clean),

                // Floor 3 -- Superior (301-308), Suite (309-310)
                R("301", 3, sup, 3, "1 King Bed", 139, RoomStatus.Occupied, HousekeepingStatus.Occupied),
                R("302", 3, sup, 3, "1 King Bed", 139, RoomStatus.Occupied, HousekeepingStatus.Occupied),
                R("303", 3, sup, 3, "1 King Bed", 139, RoomStatus.Available, HousekeepingStatus.Clean),
                R("304", 3, sup, 3, "1 King Bed", 139, RoomStatus.Reserved, HousekeepingStatus.Clean),
                R("305", 3, sup, 3, "1 King Bed", 139, RoomStatus.Occupied, HousekeepingStatus.Occupied),
                R("306", 3, sup, 3, "1 King Bed", 139, RoomStatus.Available, HousekeepingStatus.Dirty),
                R("307", 3, sup, 3, "1 King Bed", 139, RoomStatus.Available, HousekeepingStatus.Inspected),
                R("308", 3, sup, 3, "1 King Bed", 139, RoomStatus.Occupied, HousekeepingStatus.Occupied),
                R("309", 3, ste, 4, "1 King Bed + Sofa Bed", 219, RoomStatus.Available, HousekeepingStatus.Clean),
                R("310", 3, ste, 4, "1 King Bed + Sofa Bed", 219, RoomStatus.Occupied, HousekeepingStatus.Occupied),

                // Floor 4 -- Suite (401-404), Executive Suite (405-408)
                R("401", 4, ste, 4, "1 King Bed + Sofa Bed", 219, RoomStatus.Occupied, HousekeepingStatus.Occupied),
                R("402", 4, ste, 4, "1 King Bed + Sofa Bed", 219, RoomStatus.Available, HousekeepingStatus.Clean),
                R("403", 4, ste, 4, "1 King Bed + Sofa Bed", 219, RoomStatus.Reserved, HousekeepingStatus.Clean),
                R("404", 4, ste, 4, "1 King Bed + Sofa Bed", 219, RoomStatus.Occupied, HousekeepingStatus.Occupied),
                R("405", 4, exe, 4, "1 King Bed + Sofa Bed", 299, RoomStatus.Occupied, HousekeepingStatus.Occupied),
                R("406", 4, exe, 4, "1 King Bed + Sofa Bed", 299, RoomStatus.Available, HousekeepingStatus.Inspected),
                R("407", 4, exe, 4, "1 King Bed + Sofa Bed", 299, RoomStatus.UnderMaintenance, HousekeepingStatus.Dirty),
                R("408", 4, exe, 4, "1 King Bed + Sofa Bed", 299, RoomStatus.Available, HousekeepingStatus.Clean)
            };
        }
    }
}
