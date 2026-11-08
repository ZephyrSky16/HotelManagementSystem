using System.Collections.Generic;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Models.Common;

namespace Revelation.Front.Office.Core.Interfaces.Services
{
    public interface IMaintenanceService
    {
        IEnumerable<MaintenanceRequest> GetAll();
        MaintenanceRequest Create(MaintenanceRequest request);
        void UpdateStatus(int requestId, MaintenanceStatus status);
        void AssignPersonnel(int requestId, string personnel);
    }
}
