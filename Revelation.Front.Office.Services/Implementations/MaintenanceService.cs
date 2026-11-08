using System;
using System.Collections.Generic;
using System.Linq;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Services.MockData;

namespace Revelation.Front.Office.Services.Implementations
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IRoomService _roomService;

        public MaintenanceService(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public IEnumerable<MaintenanceRequest> GetAll() =>
            MockDataStore.MaintenanceRequests.OrderByDescending(m => m.ReportedDate);

        public MaintenanceRequest Create(MaintenanceRequest request)
        {
            request.Id = MockDataStore.NextId();
            var maxNum = MockDataStore.MaintenanceRequests
                .Select(m => int.TryParse(m.RequestNo.Replace("MR-", ""), out var n) ? n : 0)
                .DefaultIfEmpty(1049).Max();
            request.RequestNo = $"MR-{maxNum + 1}";
            request.ReportedDate = DateTime.Now;
            request.Status = string.IsNullOrEmpty(request.AssignedPersonnel) ? MaintenanceStatus.Reported : MaintenanceStatus.Assigned;
            MockDataStore.MaintenanceRequests.Add(request);

            if (request.MarkedRoomOutOfOrder)
            {
                var room = MockDataStore.Rooms.First(r => r.Id == request.RoomId);
                room.Status = RoomStatus.OutOfOrder;
            }
            return request;
        }

        public void UpdateStatus(int requestId, MaintenanceStatus status)
        {
            var request = MockDataStore.MaintenanceRequests.FirstOrDefault(m => m.Id == requestId);
            if (request == null) return;

            request.Status = status;
            if (status == MaintenanceStatus.Completed || status == MaintenanceStatus.Cancelled)
            {
                request.ActualCompletion = DateTime.Now;

                var room = MockDataStore.Rooms.FirstOrDefault(r => r.Id == request.RoomId);
                if (room != null && room.Status == RoomStatus.OutOfOrder)
                {
                    var stillOpen = MockDataStore.MaintenanceRequests.Any(m =>
                        m.RoomId == request.RoomId && m.Id != request.Id &&
                        m.Status != MaintenanceStatus.Completed && m.Status != MaintenanceStatus.Cancelled);

                    if (!stillOpen)
                    {
                        room.Status = RoomStatus.Available;
                        room.HousekeepingStatus = HousekeepingStatus.Dirty;
                    }
                }
            }
        }

        public void AssignPersonnel(int requestId, string personnel)
        {
            var request = MockDataStore.MaintenanceRequests.FirstOrDefault(m => m.Id == requestId);
            if (request == null) return;

            request.AssignedPersonnel = personnel;
            if (!string.IsNullOrEmpty(personnel) && request.Status == MaintenanceStatus.Reported)
            {
                request.Status = MaintenanceStatus.Assigned;
            }
        }
    }
}
