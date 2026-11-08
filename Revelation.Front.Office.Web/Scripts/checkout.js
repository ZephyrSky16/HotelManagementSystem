/* ==========================================================================
   checkout.js -- Check-out workflow, AJAX-driven against CheckOutController.
   ========================================================================== */

(function ($) {
    "use strict";

    var selectedId = null;

    function renderFolio(f) {
        var rows = "";
        rows += '<tr><td>Room Accommodation -- ' + f.roomType + ' (' + f.nights + ' night(s) @ ' + App.formatCurrency(f.rate) + ')</td><td class="text-end">' + App.formatCurrency(f.subtotal) + '</td></tr>';
        if (f.extraCharges > 0) { rows += '<tr><td>Additional Charges</td><td class="text-end">' + App.formatCurrency(f.extraCharges) + '</td></tr>'; }
        rows += '<tr><td>Taxes &amp; Fees</td><td class="text-end">' + App.formatCurrency(f.taxesAndFees) + '</td></tr>';
        if (f.discount > 0) { rows += '<tr><td>Discount</td><td class="text-end">-' + App.formatCurrency(f.discount) + '</td></tr>'; }
        $("#coChargesBody").html(rows);
    }

    function selectGuest(id) {
        $.get(window.CheckOutUrls.getFolio, { id: id }).done(function (f) {
            if (!f.found) { return; }
            selectedId = f.id;

            $(".guest-item").removeClass("bg-light");
            $('.guest-item[data-id="' + id + '"]').addClass("bg-light");

            $("#coEmptyState").hide();
            $("#coPanel").show();

            $("#coGuestName").text(f.guestName);
            $("#coResBadge").text(f.reservationNo);
            $("#coRoom").text("Room " + f.roomNumber + " -- " + f.roomType);
            $("#coDates").text(f.checkIn + " \u2192 " + f.checkOut);
            $("#coNights").text(f.nights + (f.nights === 1 ? " night" : " nights"));

            renderFolio(f);
            $("#coTotal").text(App.formatCurrency(f.totalAmount));
            $("#coPaid").text(App.formatCurrency(f.amountPaid));
            $("#coBalance").text(App.formatCurrency(f.balance));
            $("#coPaymentAmount").val("");
            $("#coChargeDesc").val("");
            $("#coChargeAmt").val("");
        }).fail(App.ajaxError);
    }

    $(document).on("click", ".guest-item", function (e) { e.preventDefault(); selectGuest($(this).data("id")); });

    $("#coSearchInput").on("input", function () {
        var q = $(this).val().trim().toLowerCase();
        $(".guest-item").each(function () { $(this).toggle($(this).text().toLowerCase().indexOf(q) !== -1); });
    });

    $("#coAddChargeBtn").on("click", function () {
        var desc = $("#coChargeDesc").val().trim();
        var amt = Number($("#coChargeAmt").val());
        if (!desc || !amt || amt <= 0) { App.toast("Enter a charge description and a valid amount.", "error"); return; }
        $.post(window.CheckOutUrls.addCharge, { reservationId: selectedId, amount: amt }).done(function () {
            App.toast("Charge added to folio.");
            selectGuest(selectedId);
        }).fail(App.ajaxError);
    });

    $("#coRecordPaymentBtn").on("click", function () {
        var amt = Number($("#coPaymentAmount").val());
        if (!amt || amt <= 0) { App.toast("Enter a valid payment amount.", "error"); return; }
        $.post(window.CheckOutUrls.recordPayment, { reservationId: selectedId, amount: amt }).done(function () {
            App.toast("Payment of " + App.formatCurrency(amt) + " recorded.");
            selectGuest(selectedId);
        }).fail(App.ajaxError);
    });

    $("#coCancelBtn").on("click", function () {
        selectedId = null;
        $("#coPanel").hide();
        $("#coEmptyState").show();
        $(".guest-item").removeClass("bg-light");
    });

    $("#coConfirmBtn").on("click", function () {
        if (!selectedId) { return; }
        App.confirmAction({ title: "Confirm Check-out", confirmText: "Confirm Check-out", html: "The room will be marked <strong>Dirty</strong> for housekeeping." }).then(function (ok) {
            if (!ok) { return; }
            $.post(window.CheckOutUrls.confirm, { reservationId: selectedId }).done(function (r) {
                if (!r.success) { App.toast(r.message, "error"); return; }
                App.toast(r.guestName + " checked out. Room " + r.roomNumber + " marked Dirty for housekeeping.");
                setTimeout(function () { location.reload(); }, 1200);
            }).fail(App.ajaxError);
        });
    });
})(jQuery);
