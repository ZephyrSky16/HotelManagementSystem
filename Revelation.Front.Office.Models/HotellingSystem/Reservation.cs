using System;

namespace Revelation.Front.Office.Models.HotellingSystem
{
    /// <summary>
    /// dbo.Reservation. The database defines no foreign keys, so the
    /// navigation properties below are relationships declared in EF only;
    /// a stale Id simply loads as a null navigation.
    /// AccountExecutiveId and SettleByGuestId have no matching table yet and
    /// are kept as plain values.
    /// </summary>
    public class Reservation : HsEntity
    {
        public int? AccountExecutiveId { get; set; }

        public int? AgentId { get; set; }
        public virtual Agent Agent { get; set; }
        public string AgentName { get; set; }

        public string CardExpiry { get; set; }
        public string CheckInRemarks { get; set; }
        public string CheckOutRemarks { get; set; }
        public bool? IsCommission { get; set; }

        public int? ConfirmToId { get; set; }
        public virtual ConfirmTo ConfirmTo { get; set; }

        public int? ConfirmViaId { get; set; }
        public virtual ConfirmVia ConfirmVia { get; set; }

        public DateTime? BookingReleaseCutOffDate { get; set; }

        public int? LastCancellationId { get; set; }
        public virtual Cancellation LastCancellation { get; set; }

        public int? RequiredDepositAmount { get; set; }
        public string GroupCoordinator { get; set; }

        public int? MarketCodeId { get; set; }
        public virtual MarketCode MarketCode { get; set; }

        public string OTABookingReference { get; set; }

        public int? RateId { get; set; }
        public virtual Rate Rate { get; set; }

        public int? ReservationStatusId { get; set; }
        public virtual ReservationStatus ReservationStatus { get; set; }

        public bool? IsSendBillAllCharges { get; set; }
        public int? SettleByGuestId { get; set; }
        public string SettlementReference { get; set; }

        public int? SalesId { get; set; }
        public virtual Sale Sales { get; set; }

        public int? BookingCodeId { get; set; }
        public virtual BookingCode BookingCode { get; set; }

        public override string DisplayText => "Reservation #" + Id;
    }
}
