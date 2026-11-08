using System.Collections.Generic;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Core.Interfaces.Services
{
    public interface IReservationService
    {
        IEnumerable<Reservation> GetAll();
        Reservation GetById(int id);
        Reservation GetByReservationNo(string reservationNo);
        IEnumerable<Reservation> GetTodaysArrivals();
        IEnumerable<Reservation> GetTodaysDepartures();
        IEnumerable<Reservation> GetInHouse();
        IEnumerable<Room> FindAvailableRooms(int roomTypeId, System.DateTime checkIn, System.DateTime checkOut, int? excludeReservationId = null);
        Reservation Create(Reservation reservation);
        void Update(Reservation reservation);
        void AssignRoom(int reservationId, int roomId);
        void CheckIn(int reservationId);
        void CheckOut(int reservationId);
        void Cancel(int reservationId);
        void RecordPayment(int reservationId, decimal amount);
    }
}
