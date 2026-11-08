using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Revelation.Front.Office.Web.HotellingSystem;
using HS = Revelation.Front.Office.Models.HotellingSystem;

namespace Revelation.Front.Office.Web.ViewModels
{
    public class MasterDataPageViewModel
    {
        public MasterDataDefinition Definition { get; set; }
        public IList<MasterDataRow> Rows { get; set; }
    }

    /// <summary>Create/edit form for dbo.Reservation. String lengths mirror the database columns.</summary>
    public class ReservationRecordFormViewModel
    {
        public int? Id { get; set; }

        [Display(Name = "Reservation Status")]
        public int? ReservationStatusId { get; set; }

        [Display(Name = "Booking Code")]
        public int? BookingCodeId { get; set; }

        [Display(Name = "Market Code")]
        public int? MarketCodeId { get; set; }

        [Display(Name = "Rate Code")]
        public int? RateId { get; set; }

        [Display(Name = "Agent")]
        public int? AgentId { get; set; }

        [Display(Name = "Agent Name"), StringLength(200)]
        public string AgentName { get; set; }

        [Display(Name = "Commissionable")]
        public bool IsCommission { get; set; }

        [Display(Name = "Sales")]
        public int? SalesId { get; set; }

        [Display(Name = "Account Executive Id"), Range(1, int.MaxValue)]
        public int? AccountExecutiveId { get; set; }

        [Display(Name = "Confirm To")]
        public int? ConfirmToId { get; set; }

        [Display(Name = "Confirm Via")]
        public int? ConfirmViaId { get; set; }

        [Display(Name = "OTA Booking Reference"), StringLength(200)]
        public string OTABookingReference { get; set; }

        [Display(Name = "Group Coordinator")]
        public string GroupCoordinator { get; set; }

        [Display(Name = "Booking Release Cut-off Date"), DataType(DataType.Date)]
        public DateTime? BookingReleaseCutOffDate { get; set; }

        [Display(Name = "Required Deposit Amount"), Range(0, int.MaxValue)]
        public int? RequiredDepositAmount { get; set; }

        [Display(Name = "Card Expiry"), StringLength(50)]
        public string CardExpiry { get; set; }

        [Display(Name = "Settle By Guest Id"), Range(1, int.MaxValue)]
        public int? SettleByGuestId { get; set; }

        [Display(Name = "Settlement Reference")]
        public string SettlementReference { get; set; }

        [Display(Name = "Send bill for all charges")]
        public bool IsSendBillAllCharges { get; set; }

        [Display(Name = "Last Cancellation")]
        public int? LastCancellationId { get; set; }

        [Display(Name = "Check-in Remarks")]
        public string CheckInRemarks { get; set; }

        [Display(Name = "Check-out Remarks")]
        public string CheckOutRemarks { get; set; }

        // Dropdown sources (populated by the controller, not posted back).
        public IEnumerable<SelectListItem> ReservationStatuses { get; set; }
        public IEnumerable<SelectListItem> BookingCodes { get; set; }
        public IEnumerable<SelectListItem> MarketCodes { get; set; }
        public IEnumerable<SelectListItem> Rates { get; set; }
        public IEnumerable<SelectListItem> Agents { get; set; }
        public IEnumerable<SelectListItem> Sales { get; set; }
        public IEnumerable<SelectListItem> ConfirmTos { get; set; }
        public IEnumerable<SelectListItem> ConfirmVias { get; set; }
        public IEnumerable<SelectListItem> Cancellations { get; set; }

        public static ReservationRecordFormViewModel FromEntity(HS.Reservation r) => new ReservationRecordFormViewModel
        {
            Id = r.Id,
            ReservationStatusId = r.ReservationStatusId, BookingCodeId = r.BookingCodeId, MarketCodeId = r.MarketCodeId,
            RateId = r.RateId, AgentId = r.AgentId, AgentName = r.AgentName, IsCommission = r.IsCommission ?? false,
            SalesId = r.SalesId, AccountExecutiveId = r.AccountExecutiveId, ConfirmToId = r.ConfirmToId, ConfirmViaId = r.ConfirmViaId,
            OTABookingReference = r.OTABookingReference, GroupCoordinator = r.GroupCoordinator,
            BookingReleaseCutOffDate = r.BookingReleaseCutOffDate, RequiredDepositAmount = r.RequiredDepositAmount,
            CardExpiry = r.CardExpiry, SettleByGuestId = r.SettleByGuestId, SettlementReference = r.SettlementReference,
            IsSendBillAllCharges = r.IsSendBillAllCharges ?? false, LastCancellationId = r.LastCancellationId,
            CheckInRemarks = r.CheckInRemarks, CheckOutRemarks = r.CheckOutRemarks
        };

        public void ApplyTo(HS.Reservation r)
        {
            r.ReservationStatusId = ReservationStatusId; r.BookingCodeId = BookingCodeId; r.MarketCodeId = MarketCodeId;
            r.RateId = RateId; r.AgentId = AgentId; r.AgentName = Clean(AgentName); r.IsCommission = IsCommission;
            r.SalesId = SalesId; r.AccountExecutiveId = AccountExecutiveId; r.ConfirmToId = ConfirmToId; r.ConfirmViaId = ConfirmViaId;
            r.OTABookingReference = Clean(OTABookingReference); r.GroupCoordinator = Clean(GroupCoordinator);
            r.BookingReleaseCutOffDate = BookingReleaseCutOffDate; r.RequiredDepositAmount = RequiredDepositAmount;
            r.CardExpiry = Clean(CardExpiry); r.SettleByGuestId = SettleByGuestId; r.SettlementReference = Clean(SettlementReference);
            r.IsSendBillAllCharges = IsSendBillAllCharges; r.LastCancellationId = LastCancellationId;
            r.CheckInRemarks = Clean(CheckInRemarks); r.CheckOutRemarks = Clean(CheckOutRemarks);
        }

        private static string Clean(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        /// <summary>
        /// Active rows plus the currently selected row even if it has since
        /// been deactivated, so editing an old reservation never silently
        /// drops its value.
        /// </summary>
        public static IEnumerable<SelectListItem> Options<T>(IEnumerable<T> all, int? selectedId) where T : HS.HsEntity =>
            all.Where(e => e.Id == selectedId || !(e is HS.IActivatable a) || a.IsActive != false)
               .Select(e => new SelectListItem
               {
                   Value = e.Id.ToString(),
                   Text = (e is HS.IActivatable a && a.IsActive == false) ? e.DisplayText + " (inactive)" : e.DisplayText
               })
               .ToList();
    }
}
