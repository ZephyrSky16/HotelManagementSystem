using System;
using System.Collections.Generic;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Services.MockData
{
    public static partial class MockDataStore
    {
        private static List<Guest> SeedGuests()
        {
            Guest G(string first, string last, string email, string phone, string nat, IdentificationType idType,
                    string idNo, string address, DateTime lastVisit, string notes = "", string requests = "") =>
                new Guest
                {
                    Id = NextId(), FirstName = first, LastName = last, Email = email, Phone = phone, Nationality = nat,
                    IdType = idType, IdNumber = idNo, Address = address, LastVisit = lastVisit,
                    InternalNotes = notes, SpecialRequests = requests
                };

            const IdentificationType P = IdentificationType.Passport;
            const IdentificationType DL = IdentificationType.DriversLicense;

            return new List<Guest>
            {
                G("Daniel", "Whitfield", "daniel.whitfield@mail.com", "+1 (415) 555-0142", "United States", P, "US48213765", "221 Harbor Ave, San Francisco, CA", new DateTime(2026, 10, 21), "Returning guest, prefers high floor rooms.", "Late check-out when possible"),
                G("Isabela", "Carvalho", "isabela.carvalho@mail.com", "+55 11 98765-4321", "Brazil", P, "BR77291034", "Rua das Flores 88, Sao Paulo", new DateTime(2026, 9, 2), "", "Vegetarian breakfast"),
                G("Marcus", "Chen", "marcus.chen@mail.com", "+1 (212) 555-0199", "United States", DL, "DL-NY5521487", "45 Lexington Ave, New York, NY", new DateTime(2026, 10, 29), "VIP -- corporate account, Chen & Associates.", "Quiet room away from elevator"),
                G("Amara", "Okafor", "amara.okafor@mail.com", "+234 803 555 0198", "Nigeria", P, "NG90218834", "14 Adeola Street, Lagos", new DateTime(2026, 8, 14)),
                G("Haruto", "Tanaka", "haruto.tanaka@mail.com", "+81 90-5551-2234", "Japan", P, "JP55123487", "3-21 Shibuya, Tokyo", new DateTime(2026, 10, 12), "", "Extra pillows, non-smoking"),
                G("Sophie", "Lindqvist", "sophie.lindqvist@mail.com", "+46 70-555-2341", "Sweden", P, "SE22198745", "Vasagatan 10, Stockholm", new DateTime(2026, 7, 30)),
                G("Rahul", "Mehta", "rahul.mehta@mail.com", "+91 98765 43210", "India", P, "IN77341298", "12 MG Road, Bengaluru", new DateTime(2026, 10, 25), "Travels frequently for business.", "Early check-in requested"),
                G("Chloe", "Dubois", "chloe.dubois@mail.com", "+33 6 12 34 56 78", "France", P, "FR66123890", "18 Rue de Rivoli, Paris", new DateTime(2026, 9, 18)),
                G("Liam", "O'Sullivan", "liam.osullivan@mail.com", "+353 87 555 2290", "Ireland", P, "IE44129876", "27 Grafton St, Dublin", new DateTime(2026, 6, 5)),
                G("Priya", "Nair", "priya.nair@mail.com", "+91 90123 45678", "India", P, "IN88213476", "9 Marine Drive, Mumbai", new DateTime(2026, 10, 30), "Frequent traveler, prefers suite upgrades when available.", "Ocean view preferred"),
                G("Oliver", "Bennett", "oliver.bennett@mail.com", "+44 7700 900142", "United Kingdom", P, "UK55129087", "5 Baker Street, London", new DateTime(2026, 10, 8), "", "Allergic to feather pillows"),
                G("Valentina", "Rossi", "valentina.rossi@mail.com", "+39 345 555 0123", "Italy", P, "IT90218765", "Via Roma 22, Milan", new DateTime(2026, 8, 27)),
                G("Ethan", "Walker", "ethan.walker@mail.com", "+1 (310) 555-0176", "United States", DL, "DL-CA3391827", "900 Sunset Blvd, Los Angeles, CA", new DateTime(2026, 10, 31), "Celebrating wedding anniversary on this stay.", "Champagne and flowers on arrival"),
                G("Grace", "Mutua", "grace.mutua@mail.com", "+254 712 345 678", "Kenya", P, "KE33219087", "Kimathi St, Nairobi", new DateTime(2026, 5, 19)),
                G("Samuel", "Nilsson", "samuel.nilsson@mail.com", "+46 73-555-9087", "Sweden", P, "SE88129076", "Drottninggatan 5, Gothenburg", new DateTime(2026, 9, 9)),
                G("Fatima", "Al-Sayed", "fatima.alsayed@mail.com", "+971 50 555 2341", "United Arab Emirates", P, "AE77129876", "Sheikh Zayed Rd, Dubai", new DateTime(2026, 10, 19), "", "Connecting rooms for family"),
                G("Noah", "Kowalski", "noah.kowalski@mail.com", "+48 601 234 567", "Poland", P, "PL99218765", "ul. Nowy Swiat 12, Warsaw", new DateTime(2026, 4, 22)),
                G("Mia", "Johansen", "mia.johansen@mail.com", "+47 912 34 567", "Norway", P, "NO66129087", "Karl Johans gate 8, Oslo", new DateTime(2026, 8, 2)),
                G("Benjamin", "Harris", "benjamin.harris@mail.com", "+1 (617) 555-0133", "United States", DL, "DL-MA2201984", "100 Boylston St, Boston, MA", new DateTime(2026, 10, 26)),
                G("Elena", "Petrova", "elena.petrova@mail.com", "+7 916 555 2341", "Russia", P, "RU55129087", "Tverskaya St 9, Moscow", new DateTime(2026, 3, 11)),
                G("James", "Thornton", "james.thornton@mail.com", "+1 (773) 555-0187", "United States", P, "US22198076", "500 Michigan Ave, Chicago, IL", new DateTime(2026, 11, 1), "Long-tenured corporate client, billing to Thornton Logistics.", "Invoice emailed to accounts department"),
                G("Yuki", "Sato", "yuki.sato@mail.com", "+81 80-5551-9087", "Japan", P, "JP22198765", "2-5 Namba, Osaka", new DateTime(2026, 9, 25)),
                G("Aisha", "Bello", "aisha.bello@mail.com", "+234 802 555 1298", "Nigeria", P, "NG55129876", "Allen Ave, Lagos", new DateTime(2026, 6, 30)),
                G("Lucas", "Fernandez", "lucas.fernandez@mail.com", "+52 55 5555 0123", "Mexico", P, "MX77129087", "Av. Reforma 222, Mexico City", new DateTime(2026, 10, 15)),
                G("Dan", "Whitfield", "daniel.whitfield@mail.com", "+1 (415) 555-0142", "United States", P, "US48213765", "221 Harbor Ave, San Francisco, CA", new DateTime(2026, 4, 2), "Possible duplicate -- same email and phone as guest #1 (Daniel Whitfield); likely created at a walk-in desk under a shortened first name.")
            };
        }
    }
}
