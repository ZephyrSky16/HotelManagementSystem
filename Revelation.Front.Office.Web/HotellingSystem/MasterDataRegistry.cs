using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Revelation.Front.Office.Core.Interfaces.HotellingSystem;
using Revelation.Front.Office.Models.HotellingSystem;
using Revelation.Front.Office.Repositories.EF.Generic;
using Revelation.Front.Office.Repositories.HotellingSystem;
using Revelation.Front.Office.Services.HotellingSystem;

namespace Revelation.Front.Office.Web.HotellingSystem
{
    public class MasterDataField
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public int MaxLength { get; set; }
        public bool Required { get; set; }
    }

    public class MasterDataDefinition
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string Singular { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public Type EntityType { get; set; }
        public IReadOnlyList<MasterDataField> Fields { get; set; }
        public bool HasActive { get; set; }
        public bool HasUpdateAudit { get; set; }
        internal Func<HotellingSystemDbContext, IMasterDataHandler> CreateHandler { get; set; }
    }

    /// <summary>
    /// Single place describing every HotellingSystem lookup table: its URL
    /// key, editable columns (lengths match the database), and which
    /// Reservation column references it. Also the composition root for the
    /// lookup services, since the project has no IoC container.
    /// </summary>
    public static class MasterDataRegistry
    {
        private static readonly Dictionary<Type, LambdaExpression> ReservationForeignKeys = new Dictionary<Type, LambdaExpression>
        {
            { typeof(Agent), Fk(r => r.AgentId) },
            { typeof(BookingCode), Fk(r => r.BookingCodeId) },
            { typeof(Cancellation), Fk(r => r.LastCancellationId) },
            { typeof(ConfirmTo), Fk(r => r.ConfirmToId) },
            { typeof(ConfirmVia), Fk(r => r.ConfirmViaId) },
            { typeof(MarketCode), Fk(r => r.MarketCodeId) },
            { typeof(Rate), Fk(r => r.RateId) },
            { typeof(ReservationStatus), Fk(r => r.ReservationStatusId) },
            { typeof(Sale), Fk(r => r.SalesId) }
            // Folio: nothing references it yet (Reservation.SettleByGuestId has no declared target).
        };

        public static readonly IReadOnlyList<MasterDataDefinition> All = new List<MasterDataDefinition>
        {
            Define<ReservationStatus>("reservation-statuses", "Reservation Statuses", "Reservation Status", "fa-flag",
                "Statuses a reservation can be in.", Field("Name", "Name", 100, true)),
            Define<Agent>("agents", "Agents", "Agent", "fa-user-secret",
                "Travel agents that book on behalf of guests.",
                Field("FirstName", "First Name", 200), Field("LastName", "Last Name", 200), Field("DisplayName", "Display Name", 200, true)),
            Define<BookingCode>("booking-codes", "Booking Codes", "Booking Code", "fa-barcode",
                "Booking codes and the source they come from.", Field("Code", "Code", 100, true), Field("Source", "Source", 200)),
            Define<MarketCode>("market-codes", "Market Codes", "Market Code", "fa-globe",
                "Market segments used to classify reservations.", Field("Code", "Code", 100, true)),
            Define<Rate>("rate-codes", "Rate Codes", "Rate Code", "fa-tag",
                "Rate codes applied to reservations.", Field("Code", "Code", 100, true)),
            Define<ConfirmTo>("confirm-to", "Confirm To", "Confirm To", "fa-user-circle",
                "Who receives the booking confirmation.", Field("Name", "Name", 100, true)),
            Define<ConfirmVia>("confirm-via", "Confirm Via", "Confirm Via", "fa-paper-plane",
                "Channels confirmations are sent through.", Field("Name", "Name", 100, true)),
            Define<Folio>("folios", "Folios", "Folio", "fa-folder-open",
                "Guest folios.",
                Field("FirstName", "First Name", 200), Field("LastName", "Last Name", 200), Field("DisplayName", "Display Name", 200, true)),
            Define<Sale>("sales", "Sales", "Sale", "fa-briefcase",
                "Sales records. The table currently has no descriptive columns, only an active flag."),
            Define<Cancellation>("cancellations", "Cancellations", "Cancellation", "fa-ban",
                "Cancellation records referenced by reservations. The table only stores who created each one and when.")
        };

        public static MasterDataDefinition Find(string key) =>
            All.FirstOrDefault(d => string.Equals(d.Key, key, StringComparison.OrdinalIgnoreCase));

        public static IMasterDataService<T> CreateService<T>(HotellingSystemDbContext db) where T : HsEntity
        {
            LambdaExpression fk;
            ReservationForeignKeys.TryGetValue(typeof(T), out fk);
            return new MasterDataService<T>(new EfRepository<T>(db), new EfRepository<Reservation>(db),
                fk == null ? null : UsedBy((Expression<Func<Reservation, int?>>)fk));
        }

        private static MasterDataDefinition Define<T>(string key, string title, string singular, string icon, string description,
            params MasterDataField[] fields) where T : HsEntity, new()
        {
            var definition = new MasterDataDefinition
            {
                Key = key, Title = title, Singular = singular, Icon = icon, Description = description,
                EntityType = typeof(T), Fields = fields,
                HasActive = typeof(IActivatable).IsAssignableFrom(typeof(T)),
                HasUpdateAudit = typeof(IUpdateAudited).IsAssignableFrom(typeof(T))
            };
            definition.CreateHandler = db => new MasterDataHandler<T>(definition, CreateService<T>(db));
            return definition;
        }

        private static MasterDataField Field(string name, string label, int maxLength, bool required = false) =>
            new MasterDataField { Name = name, Label = label, MaxLength = maxLength, Required = required };

        private static Expression<Func<Reservation, int?>> Fk(Expression<Func<Reservation, int?>> selector) => selector;

        /// <summary>Turns r =&gt; r.AgentId into id =&gt; (r =&gt; r.AgentId == id), which EF can translate.</summary>
        private static Func<int, Expression<Func<Reservation, bool>>> UsedBy(Expression<Func<Reservation, int?>> fk) =>
            id => Expression.Lambda<Func<Reservation, bool>>(
                Expression.Equal(fk.Body, Expression.Constant((int?)id, typeof(int?))), fk.Parameters);
    }
}
