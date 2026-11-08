using Revelation.Front.Office.Models.Common;

namespace Revelation.Front.Office.Models.Entities
{
    /// <summary>Per-room-type extra person / child surcharge used by rate calculations.</summary>
    public class ExtraCharge : BaseEntity
    {
        public int RoomTypeId { get; set; }
        public virtual RoomType RoomType { get; set; }

        public decimal ExtraPersonCharge { get; set; }
        public decimal ChildRate { get; set; }
    }
}
