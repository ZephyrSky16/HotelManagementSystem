using Revelation.Front.Office.Core.Interfaces;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Core.Interfaces.Repositories
{
    public interface IGuestRepository : IRepository<Guest>
    {
        Guest FindByEmailOrPhone(string email, string phone);
    }
}
