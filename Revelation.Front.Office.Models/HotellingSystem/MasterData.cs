using System;

namespace Revelation.Front.Office.Models.HotellingSystem
{
    /// <summary>dbo.Agent</summary>
    public class Agent : MasterDataEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }

        public override string DisplayText =>
            !string.IsNullOrWhiteSpace(DisplayName) ? DisplayName : (FirstName + " " + LastName).Trim();
    }

    /// <summary>dbo.Folio</summary>
    public class Folio : MasterDataEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }

        public override string DisplayText =>
            !string.IsNullOrWhiteSpace(DisplayName) ? DisplayName : (FirstName + " " + LastName).Trim();
    }

    /// <summary>dbo.BookingCode</summary>
    public class BookingCode : MasterDataEntity
    {
        public string Code { get; set; }
        public string Source { get; set; }

        public override string DisplayText =>
            string.IsNullOrWhiteSpace(Source) ? Code : Code + " (" + Source + ")";
    }

    /// <summary>dbo.MarketCode</summary>
    public class MarketCode : MasterDataEntity
    {
        public string Code { get; set; }
        public override string DisplayText => Code;
    }

    /// <summary>dbo.Rate (rate codes)</summary>
    public class Rate : MasterDataEntity
    {
        public string Code { get; set; }
        public override string DisplayText => Code;
    }

    /// <summary>dbo.ConfirmTo</summary>
    public class ConfirmTo : MasterDataEntity
    {
        public string Name { get; set; }
        public override string DisplayText => Name;
    }

    /// <summary>dbo.ConfirmVia</summary>
    public class ConfirmVia : MasterDataEntity
    {
        public string Name { get; set; }
        public override string DisplayText => Name;
    }

    /// <summary>dbo.ReservationStatus</summary>
    public class ReservationStatus : MasterDataEntity
    {
        public string Name { get; set; }
        public override string DisplayText => Name;
    }

    /// <summary>dbo.Sale -- the table currently holds only Id, IsActive and audit columns.</summary>
    public class Sale : MasterDataEntity
    {
        public override string DisplayText => "Sale #" + Id;
    }

    /// <summary>dbo.Cancellation -- Id plus insert audit only (no IsActive, no update columns).</summary>
    public class Cancellation : HsEntity, IInsertAudited
    {
        public DateTime? InsertedDate { get; set; }
        public string InsertedBy { get; set; }

        public override string DisplayText =>
            "Cancellation #" + Id + (InsertedDate.HasValue ? " (" + InsertedDate.Value.ToString("MMM d, yyyy") + ")" : "");
    }
}
