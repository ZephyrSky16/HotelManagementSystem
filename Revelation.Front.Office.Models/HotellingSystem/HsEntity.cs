using System;

namespace Revelation.Front.Office.Models.HotellingSystem
{
    /// <summary>
    /// Entities in this namespace map 1:1 onto the existing tables of the
    /// HotellingSystem SQL database (database-first: the schema is owned by
    /// the database, EF never creates or migrates it). They are deliberately
    /// kept apart from Models.Entities, which back the mock-data screens.
    /// </summary>
    public abstract class HsEntity
    {
        public int Id { get; set; }

        /// <summary>Human-readable label for lists and dropdowns. Read-only, so EF does not map it.</summary>
        public abstract string DisplayText { get; }
    }

    public interface IInsertAudited
    {
        DateTime? InsertedDate { get; set; }
        string InsertedBy { get; set; }
    }

    public interface IUpdateAudited
    {
        DateTime? UpdatedDate { get; set; }
        string UpdatedBy { get; set; }
    }

    public interface IActivatable
    {
        bool? IsActive { get; set; }
    }

    /// <summary>Shape shared by the lookup tables: Id, IsActive and the four audit columns.</summary>
    public abstract class MasterDataEntity : HsEntity, IActivatable, IInsertAudited, IUpdateAudited
    {
        public bool? IsActive { get; set; }
        public DateTime? InsertedDate { get; set; }
        public string InsertedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
