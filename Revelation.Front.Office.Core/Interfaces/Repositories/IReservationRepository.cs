using System.Collections.Generic;
using Revelation.Front.Office.Core.Interfaces;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Core.Interfaces.Repositories
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        Reservation GetByReservationNo(string reservationNo);
        IEnumerable<Reservation> GetByRoomId(int roomId);
        IEnumerable<Reservation> GetByGuestId(int guestId);
    }
}
