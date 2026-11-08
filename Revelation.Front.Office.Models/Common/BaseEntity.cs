using System;

namespace Revelation.Front.Office.Models.Common
{
    /// <summary>
    /// Common identity/audit fields shared by every persisted entity.
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
    }
}
