using Revelation.Front.Office.Core.Interfaces.Repositories;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Repositories.EF.Generic;

namespace Revelation.Front.Office.Repositories.EF
{
    public class RatePlanRepository : EfRepository<RatePlan>, IRatePlanRepository
    {
        public RatePlanRepository(RevelationDbContext context) : base(context) { }
    }
}
