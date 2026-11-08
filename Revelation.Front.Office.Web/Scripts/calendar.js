/* ==========================================================================
   calendar.js -- Room Availability Calendar (room rack), AJAX-driven.
   Rooms/reservations are fetched from CalendarController via jQuery AJAX;
   the rack-rendering and drag-drop mechanics are ported from the original
   static prototype's calendar.js.
   ========================================================================== */

(function ($) {
    "use strict";

    var COL_WIDTH = { day: 92, week: 132, month: 46 };
    var STATUS_CLASS = {
        "Tentative": "status-tentative", "Confirmed": "status-confirmed", "CheckedIn": "status-checkedin",
        "CheckedOut": "status-checkedout", "Cancelled": "status-cancelled", "NoShow": "status-noshow"
    };
    var STATUS_LABEL = {
        "Tentative": "Tentative", "Confirmed": "Confirmed", "CheckedIn": "Checked In",
        "CheckedOut": "Checked Out", "Cancelled": "Cancelled", "NoShow": "No Show"
    };

    var state = {
        view: "day",
        startDate: startOfDay(new Date()),
        filters: { roomTypeId: "", floor: "", roomStatus: "", search: "" },
        rooms: [],
        reservations: [],
        dragResId: null
    };

    function startOfDay(d) { var x = new Date(d); x.setHours(0, 0, 0, 0); return x; }
    function addDays(d, n) { var x = new Date(d); x.setDate(x.getDate() + n); return x; }
    function isoDate(d) {
        var y = d.getFullYear(), m = String(d.getMonth() + 1).padStart(2, "0"), day = String(d.getDate()).padStart(2, "0");
        return y + "-" + m + "-" + day;
    }
    function formatDate(iso) {
        var d = new Date(iso);
        return d.toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" });
    }

    function alignStart(view, date) {
        var d = startOfDay(date);
        if (view === "week") { d.setDate(d.getDate() - d.getDay()); }
        else if (view === "month") { d.setDate(1); }
        return d;
    }
    function dayCount(view) {
        if (view === "week") { return 7; }
        if (view === "month") { return new Date(state.startDate.getFullYear(), state.startDate.getMonth() + 1, 0).getDate(); }
        return 14;
    }
    function getVisibleDates() {
        var dates = []; var n = dayCount(state.view);
        for (var i = 0; i < n; i++) { dates.push(addDays(state.startDate, i)); }
        return dates;
    }
    function navigate(dir) {
        if (state.view === "month") {
            var d = new Date(state.startDate); d.setMonth(d.getMonth() + dir); state.startDate = alignStart("month", d);
        } else {
            state.startDate = addDays(state.startDate, dir * dayCount(state.view));
        }
        render();
    }

    function findRoom(id) { return state.rooms.filter(function (r) { return r.Id === id; })[0]; }
    function findReservation(id) { return state.reservations.filter(function (r) { return r.Id === id; })[0]; }

    function loadData(callback) {
        $.get(window.CalendarUrls.getRackData, {
            roomTypeId: state.filters.roomTypeId, floor: state.filters.floor,
            roomStatus: state.filters.roomStatus, search: state.filters.search
        }).done(function (data) {
            state.rooms = data.rooms;
            state.reservations = data.reservations;
            if (callback) { callback(); }
        }).fail(App.ajaxError);
    }

    function statusDotClass(status) {
        if (status === "Available") { return "available"; }
        if (status === "Occupied") { return "occupied"; }
        if (status === "Reserved") { return "reserved"; }
        if (status === "OutOfOrder" || status === "UnderMaintenance") { return "oor"; }
        return "available";
    }

    function renderHeader(dates, colWidth) {
        var tIso = isoDate(new Date());
        var html = '<tr class="rack-header-row"><th class="rack-room-col">Room / Type</th>';
        dates.forEach(function (d) {
            var iso = isoDate(d);
            var isToday = iso === tIso;
            var isWeekend = d.getDay() === 0 || d.getDay() === 6;
            html += '<th class="rack-date-col' + (isToday ? " is-today-col" : "") + (isWeekend ? " is-weekend" : "") + '" style="width:' + colWidth + 'px;min-width:' + colWidth + 'px;">';
            html += '<span class="date-day">' + d.getDate() + '</span>' + d.toLocaleDateString(undefined, { month: "short", weekday: "short" });
            html += '</th>';
        });
        html += '</tr>';
        $("#rackTable thead").html(html);
    }

    function renderBody(dates, colWidth) {
        var tIso = isoDate(new Date());
        var isoDates = dates.map(isoDate);
        var html = "";
        state.rooms.forEach(function (room) {
            var isOor = room.Status === "OutOfOrder";
            html += '<tr data-roomid="' + room.Id + '">';
            html += '<td class="rack-room-col rack-room-cell">';
            html += '<span class="rack-room-status-dot ' + statusDotClass(room.Status) + '"></span>';
            html += '<span class="rack-room-number">' + room.Number + '</span><br>';
            html += '<span class="rack-room-meta">' + room.Type + ' &middot; Floor ' + room.Floor + '</span></td>';
            isoDates.forEach(function (iso, idx) {
                var d = dates[idx];
                var isToday = iso === tIso;
                var isWeekend = d.getDay() === 0 || d.getDay() === 6;
                var cls = "rack-cell" + (isToday ? " is-today" : "") + (isWeekend ? " is-weekend" : "") + (isOor ? " is-oor" : "");
                html += '<td class="' + cls + '" style="width:' + colWidth + 'px;min-width:' + colWidth + 'px;" data-roomid="' + room.Id + '" data-date="' + iso + '"></td>';
            });
            html += '</tr>';
        });
        $("#rackTable tbody").html(html);
    }

    function placeBars(dates, colWidth) {
        var roomIds = state.rooms.map(function (r) { return r.Id; });
        var windowStart = dates[0];
        var windowEnd = addDays(dates[dates.length - 1], 1);

        state.reservations.forEach(function (res) {
            if (!res.RoomId || roomIds.indexOf(res.RoomId) === -1) { return; }
            var ci = startOfDay(new Date(res.CheckIn));
            var co = startOfDay(new Date(res.CheckOut));
            if (co <= windowStart || ci >= windowEnd) { return; }

            var effStart = ci < windowStart ? windowStart : ci;
            var effEnd = co > windowEnd ? windowEnd : co;
            var startIdx = Math.round((effStart - windowStart) / 86400000);
            var nights = Math.max(1, Math.round((effEnd - effStart) / 86400000));

            var $cell = $('.rack-cell[data-roomid="' + res.RoomId + '"][data-date="' + isoDate(effStart) + '"]');
            if (!$cell.length) { return; }

            var width = Math.max(colWidth - 4, nights * colWidth - 4);
            var label = App.escapeHtml(res.GuestName) + (colWidth >= 90 ? " &middot; " + res.ReservationNo : "");
            var tooltip = res.GuestName + "\n" + res.ReservationNo + "\nArrival: " + formatDate(res.CheckIn) + "\nDeparture: " + formatDate(res.CheckOut) + "\nStatus: " + STATUS_LABEL[res.BookingStatus];

            var draggable = res.BookingStatus !== "CheckedOut" && res.BookingStatus !== "NoShow";
            var $bar = $('<div class="booking-bar ' + (STATUS_CLASS[res.BookingStatus] || "") + '" draggable="' + draggable + '" title="' + App.escapeHtml(tooltip) + '" data-resid="' + res.Id + '" style="width:' + width + 'px;"></div>');
            $bar.html(label);
            $cell.append($bar);
        });
    }

    function render() {
        var dates = getVisibleDates();
        var colWidth = COL_WIDTH[state.view];
        loadData(function () {
            renderHeader(dates, colWidth);
            renderBody(dates, colWidth);
            placeBars(dates, colWidth);
            $("#roomCountLabel").html("Showing " + state.rooms.length + " room(s) &middot; " + dates.length + " day(s) from " + formatDate(isoDate(dates[0])) + " to " + formatDate(isoDate(dates[dates.length - 1])));
        });
    }

    // ---- Empty-slot click -> quick booking modal ---------------------------
    $(document).on("click", ".rack-cell", function (e) {
        if ($(e.target).closest(".booking-bar").length) { return; }
        var roomId = Number($(this).data("roomid"));
        var dateIso = $(this).data("date");
        var room = findRoom(roomId);
        if (!room) { return; }
        if (room.Status === "OutOfOrder") { App.toast("Room " + room.Number + " is Out of Order and cannot be booked.", "error"); return; }
        if (dateIso < isoDate(new Date())) { App.toast("Cannot create a booking on a past date.", "error"); return; }
        openQuickBookingModal(room, dateIso);
    });

    function openQuickBookingModal(room, dateIso) {
        $("#quickBookingForm")[0].reset();
        $("#qbRoomId").val(room.Id);
        $("#qbRoomLabel").text(room.Number);
        $("#qbRoomTypeLabel").text(room.Type);
        var ci = new Date(dateIso);
        var co = addDays(ci, 1);
        $("#qbCheckIn").val(isoDate(ci));
        $("#qbCheckOut").val(isoDate(co));

        if (window.flatpickr) {
            if (window._qbCiFp) { window._qbCiFp.destroy(); }
            if (window._qbCoFp) { window._qbCoFp.destroy(); }
            window._qbCiFp = flatpickr("#qbCheckIn", { dateFormat: "Y-m-d", minDate: "today", onChange: function (sel) {
                if (sel[0] && window._qbCoFp) { window._qbCoFp.set("minDate", addDays(sel[0], 1)); }
            } });
            window._qbCoFp = flatpickr("#qbCheckOut", { dateFormat: "Y-m-d", minDate: addDays(ci, 1) });
        }
        new bootstrap.Modal(document.getElementById("quickBookingModal")).show();
    }

    $("#quickBookingForm").on("submit", function (e) {
        e.preventDefault();
        var data = {
            roomId: $("#qbRoomId").val(), checkIn: $("#qbCheckIn").val(), checkOut: $("#qbCheckOut").val(),
            guestName: $("#qbGuestName").val(), phone: $("#qbGuestPhone").val(),
            adults: $("#qbAdults").val(), children: $("#qbChildren").val(), source: $("#qbSource").val()
        };
        $.post(window.CalendarUrls.createBooking, data).done(function (result) {
            if (!result.success) { App.toast(result.message, "error"); return; }
            bootstrap.Modal.getInstance(document.getElementById("quickBookingModal")).hide();
            render();
            App.toast("Booking " + result.reservationNo + " created.");
        }).fail(App.ajaxError);
    });

    // ---- Booking bar click -> details modal ---------------------------------
    $(document).on("click", ".booking-bar", function (e) {
        e.stopPropagation();
        openBookingDetails(Number($(this).data("resid")));
    });

    function infoRow(label, value) {
        return '<div class="d-flex justify-content-between py-1 border-bottom"><span class="text-muted text-xs">' + label + '</span><span class="text-end">' + value + '</span></div>';
    }

    function openBookingDetails(resId) {
        var res = findReservation(resId);
        if (!res) { return; }
        var room = findRoom(res.RoomId);

        var body = '<div class="d-flex justify-content-between align-items-start mb-3">';
        body += '<div><h5 class="mb-0">' + App.escapeHtml(res.GuestName) + '</h5><div class="text-muted text-xs">' + res.ReservationNo + '</div></div>';
        body += '<span class="status-pill ' + (STATUS_CLASS[res.BookingStatus] ? "status-info" : "status-slate") + '">' + STATUS_LABEL[res.BookingStatus] + '</span></div>';
        body += infoRow("Room", (room ? room.Number : "--") + " -- " + res.RoomType);
        body += infoRow("Check-in", formatDate(res.CheckIn));
        body += infoRow("Check-out", formatDate(res.CheckOut));
        body += infoRow("Nights", res.Nights);
        body += infoRow("Guests", res.Adults + " Adult(s)" + (res.Children ? ", " + res.Children + " Child(ren)" : ""));
        body += infoRow("Contact", res.ContactNumber || "--");
        body += '<div class="section-divider"></div>';
        body += infoRow("Total Amount", App.formatCurrency(res.TotalAmount));
        body += infoRow("Amount Paid", App.formatCurrency(res.AmountPaid));
        body += infoRow("Balance", '<span class="' + (res.Balance > 0 ? "text-danger fw-600" : "text-success fw-600") + '">' + App.formatCurrency(res.Balance) + '</span>');

        $("#bookingDetailsBody").html(body);

        var footer = '<a href="' + window.CalendarUrls.reservationsIndex + '?res=' + res.ReservationNo + '" class="btn btn-hms-outline"><i class="fa fa-pencil me-1"></i>Edit</a>';
        if (res.BookingStatus === "Confirmed" || res.BookingStatus === "Tentative") {
            footer += '<button type="button" class="btn btn-outline-danger" id="cancelResBtn" data-id="' + res.Id + '">Cancel Reservation</button>';
            footer += '<button type="button" class="btn btn-hms-primary" id="detailsCheckinBtn" data-id="' + res.Id + '"><i class="fa fa-sign-in me-1"></i>Check-in</button>';
        } else if (res.BookingStatus === "CheckedIn") {
            footer += '<a href="' + window.CalendarUrls.checkOutIndex + '?res=' + res.ReservationNo + '" class="btn btn-hms-primary"><i class="fa fa-sign-out me-1"></i>Go to Check-out</a>';
        }
        $("#bookingDetailsFooter").html(footer);
        new bootstrap.Modal(document.getElementById("bookingDetailsModal")).show();
    }

    $(document).on("click", "#detailsCheckinBtn", function () {
        var id = $(this).data("id");
        App.confirmAction({ title: "Confirm check-in?", confirmText: "Check In" }).then(function (ok) {
            if (!ok) { return; }
            $.post(window.CalendarUrls.checkIn, { reservationId: id }).done(function () {
                bootstrap.Modal.getInstance(document.getElementById("bookingDetailsModal")).hide();
                render();
                App.toast("Guest checked in.");
            }).fail(App.ajaxError);
        });
    });

    $(document).on("click", "#cancelResBtn", function () {
        var id = $(this).data("id");
        App.confirmAction({ title: "Cancel this reservation?", danger: true, confirmText: "Cancel Reservation" }).then(function (ok) {
            if (!ok) { return; }
            $.post(window.CalendarUrls.cancelReservation, { reservationId: id }).done(function () {
                bootstrap.Modal.getInstance(document.getElementById("bookingDetailsModal")).hide();
                render();
                App.toast("Reservation cancelled.", "warning");
            }).fail(App.ajaxError);
        });
    });

    // ---- Drag and drop -------------------------------------------------------
    $(document).on("dragstart", ".booking-bar", function (e) {
        state.dragResId = Number($(this).data("resid"));
        $(this).addClass("drag-ghost");
        e.originalEvent.dataTransfer.setData("text/plain", String(state.dragResId));
    });
    $(document).on("dragend", ".booking-bar", function () { $(this).removeClass("drag-ghost"); });
    $(document).on("dragover", ".rack-cell", function (e) { e.preventDefault(); });
    $(document).on("drop", ".rack-cell", function (e) {
        e.preventDefault();
        var targetRoomId = Number($(this).data("roomid"));
        var resId = state.dragResId;
        var res = findReservation(resId);
        if (!res || !targetRoomId || targetRoomId === res.RoomId) { return; }

        App.confirmAction({
            title: "Move reservation?",
            html: "Move <strong>" + App.escapeHtml(res.GuestName) + "</strong> to the selected room?",
            confirmText: "Move Reservation"
        }).then(function (ok) {
            if (!ok) { return; }
            $.post(window.CalendarUrls.moveReservation, { reservationId: resId, targetRoomId: targetRoomId })
                .done(function (result) {
                    if (!result.success) { App.toast(result.message, "error"); return; }
                    render();
                    App.toast(result.message);
                }).fail(App.ajaxError);
        });
    });

    // ---- Toolbar --------------------------------------------------------------
    function initToolbar() {
        $("#prevBtn").on("click", function () { navigate(-1); });
        $("#nextBtn").on("click", function () { navigate(1); });
        $("#todayBtn").on("click", function () { state.startDate = alignStart(state.view, new Date()); render(); });
        $("[data-view]").on("click", function () {
            $("[data-view]").removeClass("active");
            $(this).addClass("active");
            state.view = $(this).data("view");
            state.startDate = alignStart(state.view, state.startDate);
            render();
        });
        $("#filterRoomType").on("change", function () { state.filters.roomTypeId = $(this).val(); render(); });
        $("#filterFloor").on("change", function () { state.filters.floor = $(this).val(); render(); });
        $("#filterRoomStatus").on("change", function () { state.filters.roomStatus = $(this).val(); render(); });
        $("#calendarSearchInput").on("input", function () { state.filters.search = $(this).val().trim(); render(); });

        if (window.flatpickr) {
            flatpickr("#jumpDateInput", {
                dateFormat: "Y-m-d",
                onChange: function (selected) { if (selected[0]) { state.startDate = alignStart(state.view, selected[0]); render(); } }
            });
        }
    }

    $(function () { initToolbar(); render(); });
})(jQuery);
