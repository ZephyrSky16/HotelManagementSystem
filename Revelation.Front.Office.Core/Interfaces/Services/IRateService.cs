using System;
using System.Collections.Generic;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Core.Interfaces.Services
{
    public interface IRateService
    {
        IEnumerable<RatePlan> GetAll();
        RatePlan GetById(int id);
        RatePlan Create(RatePlan plan);
        void Update(RatePlan plan);
        void SetActive(int id, bool active);
        RatePlan ResolveRateForDate(int roomTypeId, DateTime date);
        ExtraCharge GetExtraCharge(int roomTypeId);
        void SaveExtraCharge(ExtraCharge extraCharge);
    }
}
