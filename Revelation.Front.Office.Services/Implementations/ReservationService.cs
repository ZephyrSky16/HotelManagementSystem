using System;
using System.Collections.Generic;
using System.Linq;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Services.MockData;

namespace Revelation.Front.Office.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly IRoomService _roomService;

        public ReservationService(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public IEnumerable<Reservation> GetAll() => MockDataStore.Reservations.OrderByDescending(r => r.CreatedDate);

        public Reservation GetById(int id) => MockDataStore.Reservations.FirstOrDefault(r => r.Id == id);

        public Reservation GetByReservationNo(string reservationNo) =>
            MockDataStore.Reservations.FirstOrDefault(r => r.ReservationNo == reservationNo);

        public IEnumerable<Reservation> GetTodaysArrivals()
        {
            var today = DateTime.Today;
            return MockDataStore.Reservations.Where(r =>
                r.CheckIn.Date == today && (r.BookingStatus == BookingStatus.Confirmed || r.BookingStatus == BookingStatus.Tentative));
        }

        public IEnumerable<Reservation> GetTodaysDepartures()
        {
            var today = DateTime.Today;
            return MockDataStore.Reservations.Where(r => r.CheckOut.Date == today && r.BookingStatus == BookingStatus.CheckedIn);
        }

        public IEnumerable<Reservation> GetInHouse() =>
            MockDataStore.Reservations.Where(r => r.BookingStatus == BookingStatus.CheckedIn)
                .OrderBy(r => r.CheckOut);

        public IEnumerable<Room> FindAvailableRooms(int roomTypeId, DateTime checkIn, DateTime checkOut, int? excludeReservationId = null)
        {
            return MockDataStore.Rooms.Where(room =>
            {
                if (room.RoomTypeId != roomTypeId) return false;
                if (room.Status == RoomStatus.OutOfOrder || room.Status == RoomStatus.UnderMaintenance || !room.Active) return false;

                var overlaps = MockDataStore.Reservations.Any(r =>
                    r.RoomId == room.Id &&
                    r.Id != excludeReservationId &&
                    r.BookingStatus != BookingStatus.Cancelled && r.BookingStatus != BookingStatus.NoShow &&
                    checkOut > r.CheckIn && checkIn < r.CheckOut);

                return !overlaps;
            });
        }

        public Reservation Create(Reservation reservation)
        {
            reservation.Id = MockDataStore.NextId();
            if (string.IsNullOrEmpty(reservation.ReservationNo))
            {
                var seq = 20000 + MockDataStore.Reservations.Count + new Random().Next(1, 999);
                reservation.ReservationNo = $"RES-{seq}";
            }
            reservation.CreatedDate = DateTime.Now;
            MockDataStore.Reservations.Add(reservation);

            if (reservation.RoomId.HasValue)
            {
                _roomService.RecomputeStatus(reservation.RoomId.Value);
            }
            return reservation;
        }

        public void Update(Reservation reservation)
        {
            var existing = GetById(reservation.Id);
            if (existing == null) return;

            var oldRoomId = existing.RoomId;

            existing.GuestId = reservation.GuestId;
            existing.RoomTypeId = reservation.RoomTypeId;
            existing.RoomId = reservation.RoomId;
            existing.CheckIn = reservation.CheckIn;
            existing.CheckOut = reservation.CheckOut;
            existing.Adults = reservation.Adults;
            existing.Children = reservation.Children;
            existing.Rate = reservation.Rate;
            existing.Subtotal = reservation.Subtotal;
            existing.ExtraCharges = reservation.ExtraCharges;
            existing.TaxesAndFees = reservation.TaxesAndFees;
            existing.Discount = reservation.Discount;
            existing.TotalAmount = reservation.TotalAmount;
            existing.AmountPaid = reservation.AmountPaid;
            existing.Balance = reservation.Balance;
            existing.PaymentMethod = reservation.PaymentMethod;
            existing.PaymentStatus = reservation.PaymentStatus;
            existing.PaymentReference = reservation.PaymentReference;
            existing.SpecialRequests = reservation.SpecialRequests;
            existing.StaffNotes = reservation.StaffNotes;
            existing.BookingSource = reservation.BookingSource;
            existing.ModifiedDate = DateTime.Now;

            if (oldRoomId.HasValue && oldRoomId != existing.RoomId)
            {
                _roomService.RecomputeStatus(oldRoomId.Value);
            }
            if (existing.RoomId.HasValue)
            {
                _roomService.RecomputeStatus(existing.RoomId.Value);
            }
        }

        public void AssignRoom(int reservationId, int roomId)
        {
            var reservation = GetById(reservationId);
            if (reservation == null) return;

            var oldRoomId = reservation.RoomId;
            var room = MockDataStore.Rooms.First(r => r.Id == roomId);

            reservation.RoomId = roomId;
            reservation.RoomTypeId = room.RoomTypeId;
            reservation.ModifiedDate = DateTime.Now;

            if (oldRoomId.HasValue) _roomService.RecomputeStatus(oldRoomId.Value);
            _roomService.RecomputeStatus(roomId);
        }

        public void CheckIn(int reservationId)
        {
            var reservation = GetById(reservationId);
            if (reservation?.RoomId == null) return;

            reservation.BookingStatus = BookingStatus.CheckedIn;
            reservation.ModifiedDate = DateTime.Now;
            _roomService.RecomputeStatus(reservation.RoomId.Value);
        }

        public void CheckOut(int reservationId)
        {
            var reservation = GetById(reservationId);
            if (reservation?.RoomId == null) return;

            reservation.BookingStatus = BookingStatus.CheckedOut;
            reservation.ModifiedDate = DateTime.Now;

            var room = MockDataStore.Rooms.First(r => r.Id == reservation.RoomId.Value);
            room.Status = RoomStatus.Available;
            room.HousekeepingStatus = HousekeepingStatus.Dirty;
        }

        public void Cancel(int reservationId)
        {
            var reservation = GetById(reservationId);
            if (reservation == null) return;

            reservation.BookingStatus = BookingStatus.Cancelled;
            if (reservation.AmountPaid > 0) reservation.PaymentStatus = PaymentStatus.Refunded;
            reservation.ModifiedDate = DateTime.Now;

            if (reservation.RoomId.HasValue) _roomService.RecomputeStatus(reservation.RoomId.Value);
        }

        public void RecordPayment(int reservationId, decimal amount)
        {
            var reservation = GetById(reservationId);
            if (reservation == null || amount <= 0) return;

            reservation.AmountPaid = Math.Round(reservation.AmountPaid + amount, 2);
            reservation.Balance = Math.Max(0, Math.Round(reservation.TotalAmount - reservation.AmountPaid, 2));
            reservation.PaymentStatus = reservation.Balance <= 0 ? PaymentStatus.Paid : PaymentStatus.PartiallyPaid;
        }
    }
}
