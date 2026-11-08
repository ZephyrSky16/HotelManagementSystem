using Revelation.Front.Office.Core.Interfaces;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Core.Interfaces.Repositories
{
    public interface IMaintenanceRequestRepository : IRepository<MaintenanceRequest>
    {
        string GenerateNextRequestNo();
    }
}
