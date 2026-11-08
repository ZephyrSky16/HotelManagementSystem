using Revelation.Front.Office.Core.Interfaces.Repositories;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Repositories.EF.Generic;

namespace Revelation.Front.Office.Repositories.EF
{
    public class HousekeepingLogRepository : EfRepository<HousekeepingLogEntry>, IHousekeepingLogRepository
    {
        public HousekeepingLogRepository(RevelationDbContext context) : base(context) { }
    }
}
