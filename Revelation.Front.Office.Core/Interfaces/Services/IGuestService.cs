using System.Linq;
using System.Collections.Generic;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Core.Interfaces.Services
{
    public interface IGuestService
    {
        IEnumerable<Guest> GetAll();
        Guest GetById(int id);
        Guest Create(Guest guest);
        void Update(Guest guest);
        IEnumerable<IGrouping<string, Guest>> FindPossibleDuplicates();
    }
}
