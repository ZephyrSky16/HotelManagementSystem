using System;
using System.Collections.Generic;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Services.MockData
{
    public static partial class MockDataStore
    {
        private static int _resSeq = 20200;

        private static List<Reservation> SeedReservations(List<Room> rooms, List<Guest> guests)
        {
            var list = new List<Reservation>();

            Reservation Mk(int guestIndex, string roomNumber, int ci, int co, int adults, int children,
                           BookingStatus status, PaymentStatus paymentStatus, BookingSource source,
                           int? created = null, string notes = "", string requestsOverride = null)
            {
                var guest = GuestByIndex(guests, guestIndex);
                var room = RoomByNumber(rooms, roomNumber);
                var checkIn = AddDays(ci);
                var checkOut = AddDays(co);
                var nights = Math.Max(1, (checkOut - checkIn).Days);
                var rate = room.BaseRate;
                var subtotal = rate * nights;
                var tax = Math.Round(subtotal * 0.12m, 2);
                var total = Math.Round(subtotal + tax, 2);

                decimal amountPaid;
                switch (paymentStatus)
                {
                    case PaymentStatus.Paid: amountPaid = total; break;
                    case PaymentStatus.PartiallyPaid: amountPaid = Math.Round(total * 0.4m, 2); break;
                    default: amountPaid = 0; break;
                }
                var balance = Math.Round(total - amountPaid, 2);

                _resSeq += 7;
                var reservationNo = $"RES-{_resSeq}";

                var reservation = new Reservation
                {
                    Id = NextId(),
                    ReservationNo = reservationNo,
                    GuestId = guest.Id,
                    Guest = guest,
                    RoomTypeId = room.RoomTypeId,
                    RoomType = room.RoomType,
                    RoomId = room.Id,
                    Room = room,
                    CheckIn = checkIn,
                    CheckOut = checkOut,
                    Adults = adults,
                    Children = children,
                    NumRooms = 1,
                    Rate = rate,
                    Subtotal = subtotal,
                    TaxesAndFees = tax,
                    ExtraCharges = 0,
                    Discount = 0,
                    TotalAmount = total,
                    AmountPaid = amountPaid,
                    Balance = balance,
                    PaymentMethod = PaymentMethod.CreditCard,
                    PaymentStatus = paymentStatus,
                    PaymentReference = paymentStatus != PaymentStatus.Unpaid ? $"PAY-{10000 + _resSeq}" : "",
                    BookingStatus = status,
                    BookingSource = source,
                    SpecialRequests = requestsOverride ?? guest.SpecialRequests,
                    StaffNotes = notes,
                    CreatedDate = AddDays(created ?? ci - 5)
                };
                list.Add(reservation);
                return reservation;
            }

            const BookingStatus Tent = BookingStatus.Tentative;
            const BookingStatus Conf = BookingStatus.Confirmed;
            const BookingStatus In = BookingStatus.CheckedIn;
            const BookingStatus Out = BookingStatus.CheckedOut;
            const BookingStatus Canc = BookingStatus.Cancelled;
            const BookingStatus NoShow = BookingStatus.NoShow;

            const PaymentStatus Paid = PaymentStatus.Paid;
            const PaymentStatus Partial = PaymentStatus.PartiallyPaid;
            const PaymentStatus Unpaid = PaymentStatus.Unpaid;
            const PaymentStatus Refunded = PaymentStatus.Refunded;

            const BookingSource Web = BookingSource.Website;
            const BookingSource Walk = BookingSource.WalkIn;
            const BookingSource Ph = BookingSource.Phone;
            const BookingSource TA = BookingSource.TravelAgency;
            const BookingSource OTA = BookingSource.OTA;

            // ---- Currently in-house (Checked In) -- 15, matching Occupied rooms ----
            Mk(13, "102", -2, 1, 2, 0, In, Paid, Web, -10, "Anniversary stay -- amenities arranged on arrival.");
            Mk(7, "103", -1, 2, 1, 0, In, Partial, TA, -6);
            Mk(17, "107", -3, 0, 1, 0, In, Paid, Ph, -9);
            Mk(16, "110", -1, 3, 2, 2, In, Paid, OTA, -14);
            Mk(2, "201", -2, 0, 2, 0, In, Paid, Web, -8);
            Mk(19, "204", -1, 2, 2, 0, In, Partial, Walk, -1);
            Mk(5, "207", -4, 1, 1, 0, In, Paid, Web, -12);
            Mk(10, "301", -2, 2, 2, 0, In, Paid, OTA, -20);
            Mk(3, "302", -5, 1, 1, 0, In, Paid, Ph, -30, "Corporate account -- Chen & Associates. Invoice to company.");
            Mk(11, "305", -1, 0, 1, 0, In, Paid, Web, -5);
            Mk(24, "308", -3, 1, 2, 0, In, Partial, TA, -11);
            Mk(21, "310", -6, 2, 1, 0, In, Paid, Ph, -45, "Long-tenured corporate client. Bill to Thornton Logistics.");
            Mk(6, "401", -2, 1, 2, 0, In, Paid, Web, -9);
            Mk(12, "404", -1, 2, 2, 0, In, Partial, OTA, -4);
            Mk(4, "405", -3, 1, 1, 0, In, Paid, Web, -16);

            // ---- Arriving today / upcoming confirmed (matches Reserved rooms) ----
            Mk(9, "105", 0, 2, 2, 0, Conf, Partial, Web, -7, "", "Early check-in requested");
            Mk(8, "203", 0, 3, 2, 0, Conf, Paid, OTA, -10);
            Mk(23, "210", 1, 4, 2, 1, Conf, Partial, Web, -3);
            Mk(15, "304", 0, 1, 1, 0, Conf, Paid, Ph, -2);
            Mk(18, "403", 2, 5, 2, 0, Conf, Partial, TA, -5);

            // ---- Future bookings in currently-available rooms (calendar richness) ----
            Mk(14, "101", 2, 5, 2, 0, Conf, Unpaid, Walk, 0);
            Mk(20, "104", 4, 6, 1, 0, Tent, Unpaid, Ph, -1);
            Mk(22, "109", 3, 7, 2, 2, Conf, Partial, Web, -4);
            Mk(1, "202", 1, 3, 2, 0, Conf, Paid, Web, -6);
            Mk(17, "206", 5, 6, 1, 0, Conf, Unpaid, OTA, -2);
            Mk(2, "209", 3, 5, 2, 0, Conf, Partial, Web, -1);
            Mk(23, "303", 4, 6, 2, 0, Tent, Unpaid, Ph, 0);
            Mk(9, "306", 6, 8, 2, 0, Conf, Unpaid, Web, -1);
            Mk(19, "408", 7, 9, 2, 0, Conf, Partial, OTA, -2);

            // ---- No-show / cancelled / recently checked-out singles ----
            Mk(18, "307", -2, -1, 1, 0, NoShow, Unpaid, Web, -8);
            Mk(8, "402", 1, 3, 2, 0, Canc, Refunded, OTA, -3, "Guest cancelled due to travel change; deposit refunded.");
            Mk(5, "406", -10, -7, 1, 0, Out, Paid, Web, -18);

            // ---- Purely historical reservations (guest history / table pagination) ----
            Mk(21, "310", -45, -42, 1, 0, Out, Paid, Ph, -60);
            Mk(3, "302", -40, -37, 1, 0, Out, Paid, Ph, -55);
            Mk(13, "401", -35, -32, 2, 0, Out, Paid, Web, -50);
            Mk(10, "309", -28, -25, 2, 0, Out, Paid, OTA, -40);
            Mk(7, "201", -22, -19, 1, 0, Out, Paid, TA, -35);
            Mk(16, "109", -18, -15, 2, 1, Out, Paid, Web, -28);
            Mk(11, "303", -16, -14, 1, 0, Canc, Refunded, Ph, -22);
            Mk(24, "104", -12, -9, 2, 0, Out, Paid, OTA, -20);
            Mk(6, "206", -9, -6, 2, 0, Out, Paid, Web, -15);
            Mk(12, "101", -6, -4, 2, 0, NoShow, Unpaid, Ph, -10);

            return list;
        }
    }
}
