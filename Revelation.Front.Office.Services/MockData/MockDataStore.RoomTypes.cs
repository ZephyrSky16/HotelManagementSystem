using System.Collections.Generic;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Services.MockData
{
    public static partial class MockDataStore
    {
        private static List<RoomType> SeedRoomTypes()
        {
            RoomType T(string name, string desc, int occ, int adults, int children, string bed, decimal rate, string amenities) =>
                new RoomType
                {
                    Id = NextId(), Name = name, Description = desc, MaxOccupancy = occ, MaxAdults = adults,
                    MaxChildren = children, BedConfiguration = bed, BaseRate = rate, AmenitiesCsv = amenities, Active = true
                };

            return new List<RoomType>
            {
                T("Standard Room",
                  "A comfortable, well-appointed room ideal for solo travelers and couples, featuring a queen bed and city-facing window.",
                  2, 2, 1, "1 Queen Bed", 89.00m, "Free WiFi,32\" TV,Air Conditioning,Private Bathroom"),

                T("Deluxe Room",
                  "A spacious upgrade from Standard with a king bed, work desk, and partial city views -- popular with business travelers.",
                  3, 2, 2, "1 King Bed", 119.00m, "Free WiFi,43\" Smart TV,Mini Fridge,Work Desk,City View"),

                T("Superior Room",
                  "Enhanced comfort with premium bedding, a soaking bathtub, and upgraded in-room amenities.",
                  3, 2, 2, "1 King Bed or 2 Twin Beds", 139.00m, "Free WiFi,Bathtub,Coffee Maker,Premium Toiletries,Bathrobe"),

                T("Family Room",
                  "Generously sized room with two queen beds and a sofa bed, designed for families traveling with children.",
                  5, 3, 3, "2 Queen Beds + Sofa Bed", 159.00m, "Free WiFi,Two TVs,Mini Fridge,Kids Amenities,Extra Storage"),

                T("Suite",
                  "A separate living area paired with a king bedroom, a jacuzzi tub, and views of the harbor.",
                  4, 3, 2, "1 King Bed + Sofa Bed", 219.00m, "Free WiFi,Separate Living Room,Jacuzzi,Ocean View,Nespresso Machine"),

                T("Executive Suite",
                  "The hotel's premier accommodation with panoramic harbor views, executive lounge access, and dedicated butler service.",
                  4, 3, 2, "1 King Bed + Sofa Bed", 299.00m, "Free WiFi,Executive Lounge Access,Butler Service,Panoramic View,Private Balcony,Jacuzzi")
            };
        }
    }
}
