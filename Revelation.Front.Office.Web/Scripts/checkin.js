/* ==========================================================================
   checkin.js -- Check-in workflow, AJAX-driven against CheckInController.
   ========================================================================== */

(function ($) {
    "use strict";

    var selectedId = null;

    function selectReservation(id) {
        $.get(window.CheckInUrls.getReservation, { id: id }).done(function (r) {
            if (!r.found) { return; }
            selectedId = r.id;

            $(".arrival-item").removeClass("bg-light");
            $('.arrival-item[data-id="' + id + '"]').addClass("bg-light");

            $("#ciEmptyState").hide();
            $("#ciPanel").show();

            $("#ciGuestName").text(r.guestName);
            $("#ciResBadge").text(r.reservationNo);
            $("#ciPhone").val(r.phone);
            $("#ciEmail").val(r.email);
            $("#ciNationality").val(r.nationality);
            $("#ciIdType").val(r.idType || "--");
            $("#ciIdNumber").val(r.idNumber || "");
            $("#ciIdVerified").prop("checked", false);

            $("#ciRoomNumber").text(r.roomNumber ? "Room " + r.roomNumber : "Unassigned");
            $("#ciRoomMeta").text(r.roomNumber ? r.roomType : "Please assign a room before check-in");
            $("#ciRoomPicker").hide().empty();

            $("#ciDates").text(r.checkIn + " \u2192 " + r.checkOut);
            $("#ciNights").text(r.nights + (r.nights === 1 ? " night" : " nights"));
            $("#ciGuests").text(r.adults + " Adult(s)" + (r.children ? ", " + r.children + " Child(ren)" : ""));

            $("#ciTotal").text(App.formatCurrency(r.totalAmount));
            $("#ciPaid").text(App.formatCurrency(r.amountPaid));
            $("#ciBalance").text(App.formatCurrency(r.balance));
            $("#ciPaymentAmount").val("");
        }).fail(App.ajaxError);
    }

    $(document).on("click", ".arrival-item", function (e) { e.preventDefault(); selectReservation($(this).data("id")); });

    $("#ciSearchInput").on("input", function () {
        var q = $(this).val().trim().toLowerCase();
        $(".arrival-item").each(function () {
            var text = $(this).text().toLowerCase();
            $(this).toggle(text.indexOf(q) !== -1);
        });
    });

    $("#ciChangeRoomLink").on("click", function (e) {
        e.preventDefault();
        if (!selectedId) { return; }
        $.get(window.CheckInUrls.getAlternateRooms, { reservationId: selectedId }).done(function (result) {
            if (!result.rooms.length) { App.toast("No alternate rooms of this type are available for these dates.", "warning"); return; }
            var html = result.rooms.map(function (r) {
                return '<button type="button" class="btn btn-sm btn-hms-outline me-2 mb-2 room-choice-btn" data-id="' + r.id + '">Room ' + r.number + ' (Floor ' + r.floor + ')</button>';
            }).join("");
            $("#ciRoomPicker").html(html).show();
        }).fail(App.ajaxError);
    });

    $(document).on("click", ".room-choice-btn", function () {
        var roomId = $(this).data("id");
        $.post(window.CheckInUrls.assignRoom, { reservationId: selectedId, roomId: roomId }).done(function () {
            App.toast("Room reassigned.");
            selectReservation(selectedId);
        }).fail(App.ajaxError);
    });

    $("#ciRecordPaymentBtn").on("click", function () {
        var amt = Number($("#ciPaymentAmount").val());
        if (!amt || amt <= 0) { App.toast("Enter a valid payment amount.", "error"); return; }
        $.post(window.CheckInUrls.recordPayment, { reservationId: selectedId, amount: amt }).done(function (r) {
            App.toast("Payment of " + App.formatCurrency(amt) + " recorded.");
            selectReservation(selectedId);
        }).fail(App.ajaxError);
    });

    $("#ciCancelBtn").on("click", function () {
        selectedId = null;
        $("#ciPanel").hide();
        $("#ciEmptyState").show();
        $(".arrival-item").removeClass("bg-light");
    });

    $("#ciConfirmBtn").on("click", function () {
        if (!selectedId) { return; }
        if (!$("#ciIdVerified").is(":checked")) { App.toast("Please verify the guest's identification before check-in.", "error"); return; }

        App.confirmAction({ title: "Confirm Check-in", confirmText: "Confirm Check-in" }).then(function (ok) {
            if (!ok) { return; }
            $.post(window.CheckInUrls.confirm, {
                reservationId: selectedId, phone: $("#ciPhone").val(), email: $("#ciEmail").val(),
                nationality: $("#ciNationality").val(), idNumber: $("#ciIdNumber").val()
            }).done(function (r) {
                if (!r.success) { App.toast(r.message, "error"); return; }
                App.toast(r.guestName + " successfully checked in to Room " + r.roomNumber + ".");
                setTimeout(function () { location.reload(); }, 1200);
            }).fail(App.ajaxError);
        });
    });
})(jQuery);
