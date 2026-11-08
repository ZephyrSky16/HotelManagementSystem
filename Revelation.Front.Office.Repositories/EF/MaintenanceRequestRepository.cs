using System.Linq;
using Revelation.Front.Office.Core.Interfaces.Repositories;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Repositories.EF.Generic;

namespace Revelation.Front.Office.Repositories.EF
{
    public class MaintenanceRequestRepository : EfRepository<MaintenanceRequest>, IMaintenanceRequestRepository
    {
        public MaintenanceRequestRepository(RevelationDbContext context) : base(context) { }

        public string GenerateNextRequestNo()
        {
            var max = Set.Select(m => m.Id).DefaultIfEmpty(0).Max();
            return $"MR-{1000 + max + 1}";
        }
    }
}
