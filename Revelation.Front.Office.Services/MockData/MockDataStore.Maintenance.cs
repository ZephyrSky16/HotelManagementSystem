using System.Collections.Generic;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Services.MockData
{
    public static partial class MockDataStore
    {
        private static List<MaintenanceRequest> SeedMaintenanceRequests(List<Room> rooms)
        {
            MaintenanceRequest Req(string no, string roomNo, MaintenanceCategory cat, string desc, MaintenancePriority pr,
                                    string reportedBy, string assigned, int reportedOffset, int? expectedOffset, int? actualOffset, MaintenanceStatus status) =>
                new MaintenanceRequest
                {
                    Id = NextId(), RequestNo = no, Room = RoomByNumber(rooms, roomNo), RoomId = RoomByNumber(rooms, roomNo).Id,
                    Category = cat, Description = desc, Priority = pr, ReportedBy = reportedBy, AssignedPersonnel = assigned,
                    ReportedDate = AddDays(reportedOffset),
                    ExpectedCompletion = expectedOffset.HasValue ? AddDays(expectedOffset.Value) : (System.DateTime?)null,
                    ActualCompletion = actualOffset.HasValue ? AddDays(actualOffset.Value) : (System.DateTime?)null,
                    Status = status
                };

            return new List<MaintenanceRequest>
            {
                Req("MR-1042", "108", MaintenanceCategory.AirConditioning, "AC unit not cooling, compressor making loud noise.", MaintenancePriority.High, "Front Desk -- M. Santos", "Carlos Jimenez", -2, 0, null, MaintenanceStatus.InProgress),
                Req("MR-1043", "208", MaintenanceCategory.Electrical, "Power outlet near desk sparked when guest plugged in laptop charger. Room taken out of service.", MaintenancePriority.Critical, "Housekeeping -- A. Cruz", "Derek Osei", -1, 0, null, MaintenanceStatus.Assigned),
                Req("MR-1044", "407", MaintenanceCategory.Plumbing, "Bathroom sink draining very slowly, possible clog.", MaintenancePriority.Medium, "Front Desk -- M. Santos", "Priya Raman", -1, 1, null, MaintenanceStatus.InProgress),
                Req("MR-1035", "205", MaintenanceCategory.Furniture, "Desk chair armrest broken.", MaintenancePriority.Low, "Housekeeping -- T. Gomez", "Will Harrington", -6, -4, -4, MaintenanceStatus.Completed),
                Req("MR-1036", "309", MaintenanceCategory.InternetNetwork, "In-room WiFi access point offline, guest reported no signal.", MaintenancePriority.High, "Guest -- Room 309", "Tomas Novak", -5, -5, -5, MaintenanceStatus.Completed),
                Req("MR-1037", "109", MaintenanceCategory.Structural, "Ceiling water stain noticed near window, needs inspection.", MaintenancePriority.Medium, "Housekeeping -- L. Park", "Carlos Jimenez", -9, -6, -6, MaintenanceStatus.Completed),
                Req("MR-1038", "303", MaintenanceCategory.Appliances, "Mini-fridge not cooling properly.", MaintenancePriority.Medium, "Front Desk -- J. Alvarez", "Derek Osei", -8, -5, -5, MaintenanceStatus.Completed),
                Req("MR-1039", "110", MaintenanceCategory.GeneralMaintenance, "Curtain rail detached on one side.", MaintenancePriority.Low, "Housekeeping -- A. Cruz", "Will Harrington", -7, -5, -5, MaintenanceStatus.Completed),
                Req("MR-1040", "402", MaintenanceCategory.Electrical, "Bedside lamp flickering intermittently.", MaintenancePriority.Low, "Guest -- Room 402", "Priya Raman", -4, -2, -2, MaintenanceStatus.Completed),
                Req("MR-1041", "206", MaintenanceCategory.Plumbing, "Shower head pressure very low.", MaintenancePriority.Medium, "Housekeeping -- T. Gomez", null, -3, 0, null, MaintenanceStatus.Reported),
                Req("MR-1030", "301", MaintenanceCategory.AirConditioning, "Thermostat display unresponsive.", MaintenancePriority.Medium, "Guest -- Room 301", "Carlos Jimenez", -14, -11, -12, MaintenanceStatus.Completed),
                Req("MR-1031", "107", MaintenanceCategory.Furniture, "Wardrobe door hinge loose.", MaintenancePriority.Low, "Housekeeping -- L. Park", "Will Harrington", -12, -9, null, MaintenanceStatus.Cancelled),
                Req("MR-1032", "210", MaintenanceCategory.InternetNetwork, "Smart TV unable to connect to hotel WiFi for streaming.", MaintenancePriority.Low, "Guest -- Room 210", "Tomas Novak", -10, -7, -7, MaintenanceStatus.Completed),
                Req("MR-1033", "405", MaintenanceCategory.Structural, "Balcony door seal letting in draft.", MaintenancePriority.High, "Front Desk -- M. Santos", "Derek Osei", -3, -1, null, MaintenanceStatus.OnHold)
            };
        }
    }
}
