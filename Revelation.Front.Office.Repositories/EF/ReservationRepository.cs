using System.Collections.Generic;
using System.Linq;
using Revelation.Front.Office.Core.Interfaces.Repositories;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Repositories.EF.Generic;

namespace Revelation.Front.Office.Repositories.EF
{
    public class ReservationRepository : EfRepository<Reservation>, IReservationRepository
    {
        public ReservationRepository(RevelationDbContext context) : base(context) { }

        public Reservation GetByReservationNo(string reservationNo) =>
            Set.FirstOrDefault(r => r.ReservationNo == reservationNo);

        public IEnumerable<Reservation> GetByRoomId(int roomId) =>
            Set.Where(r => r.RoomId == roomId).ToList();

        public IEnumerable<Reservation> GetByGuestId(int guestId) =>
            Set.Where(r => r.GuestId == guestId).ToList();
    }
}
