using System.Linq;
using Revelation.Front.Office.Core.Interfaces.Repositories;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Repositories.EF.Generic;

namespace Revelation.Front.Office.Repositories.EF
{
    public class RoomRepository : EfRepository<Room>, IRoomRepository
    {
        public RoomRepository(RevelationDbContext context) : base(context) { }

        public Room GetByNumber(string number) => Set.FirstOrDefault(r => r.Number == number);
    }
}
