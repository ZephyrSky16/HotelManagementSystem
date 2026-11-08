using System.Linq;
using Revelation.Front.Office.Core.Interfaces.Repositories;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Repositories.EF.Generic;

namespace Revelation.Front.Office.Repositories.EF
{
    public class GuestRepository : EfRepository<Guest>, IGuestRepository
    {
        public GuestRepository(RevelationDbContext context) : base(context) { }

        public Guest FindByEmailOrPhone(string email, string phone) =>
            Set.FirstOrDefault(g => g.Email == email || g.Phone == phone);
    }
}
