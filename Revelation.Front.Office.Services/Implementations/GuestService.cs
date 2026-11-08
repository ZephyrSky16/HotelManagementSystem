using System.Collections.Generic;
using System.Linq;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Services.MockData;

namespace Revelation.Front.Office.Services.Implementations
{
    public class GuestService : IGuestService
    {
        public IEnumerable<Guest> GetAll() => MockDataStore.Guests.OrderBy(g => g.LastName);

        public Guest GetById(int id) => MockDataStore.Guests.FirstOrDefault(g => g.Id == id);

        public Guest Create(Guest guest)
        {
            guest.Id = MockDataStore.NextId();
            MockDataStore.Guests.Add(guest);
            return guest;
        }

        public void Update(Guest guest)
        {
            var existing = GetById(guest.Id);
            if (existing == null) return;

            existing.FirstName = guest.FirstName;
            existing.LastName = guest.LastName;
            existing.Email = guest.Email;
            existing.Phone = guest.Phone;
            existing.Address = guest.Address;
            existing.Nationality = guest.Nationality;
            existing.IdType = guest.IdType;
            existing.IdNumber = guest.IdNumber;
            existing.InternalNotes = guest.InternalNotes;
            existing.SpecialRequests = guest.SpecialRequests;
        }

        /// <summary>Groups guests sharing the same email or phone -- surfaces likely duplicate profiles.</summary>
        public IEnumerable<IGrouping<string, Guest>> FindPossibleDuplicates()
        {
            var byKey = new List<Guest>();
            var groups = MockDataStore.Guests
                .SelectMany(g => new[] { (Key: g.Email, Guest: g), (Key: g.Phone, Guest: g) })
                .Where(x => !string.IsNullOrEmpty(x.Key))
                .GroupBy(x => x.Key, x => x.Guest)
                .Where(g => g.Select(x => x.Id).Distinct().Count() > 1);

            return groups;
        }
    }
}
