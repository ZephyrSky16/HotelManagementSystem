/* ==========================================================================
   rates.js -- Rate plan CRUD, extra charges, and AJAX-driven rate calendar.
   ========================================================================== */

(function ($) {
    "use strict";

    var calMonth = new Date(new Date().getFullYear(), new Date().getMonth(), 1);

    function resetForm() {
        $("#rateForm")[0].reset();
        $("#rfId").val("");
        $("#rfActive").prop("checked", true);
    }

    $("#addRateBtn").on("click", function () {
        resetForm();
        $("#rateFormTitle").text("Add Rate Plan");
        new bootstrap.Modal(document.getElementById("rateFormModal")).show();
    });

    $(document).on("click", ".edit-rate-btn", function () {
        var d = $(this).data();
        resetForm();
        $("#rfId").val(d.id);
        $("#rateFormTitle").text("Edit Rate Plan -- " + d.name);
        $("#rfRoomType").val(d.roomtypeid);
        $("#rfPlanType").val(d.type);
        $("#rfPlanName").val(d.name);
        $("#rfStart").val(d.start);
        $("#rfEnd").val(d.end);
        $("#rfDailyRate").val(d.rate);
        $("#rfMinStay").val(d.min);
        $("#rfMaxStay").val(d.max);
        $("#rfActive").prop("checked", d.active === true || d.active === "true");
        new bootstrap.Modal(document.getElementById("rateFormModal")).show();
    });

    $("#rateForm").on("submit", function (e) {
        e.preventDefault();
        var data = {
            id: $("#rfId").val() || null, roomTypeId: $("#rfRoomType").val(), ratePlanName: $("#rfPlanName").val().trim(),
            ratePlanType: $("#rfPlanType").val(), effectiveStart: $("#rfStart").val(), effectiveEnd: $("#rfEnd").val(),
            dailyRate: $("#rfDailyRate").val(), minStay: $("#rfMinStay").val(), maxStay: $("#rfMaxStay").val(), active: $("#rfActive").is(":checked")
        };
        $.post(window.RatesUrls.save, data).done(function (r) {
            if (!r.success) { App.toast(r.message, "error"); return; }
            App.toast("Rate plan saved.");
            bootstrap.Modal.getInstance(document.getElementById("rateFormModal")).hide();
            location.reload();
        }).fail(App.ajaxError);
    });

    $(document).on("click", ".toggle-rate-btn", function () {
        var id = $(this).data("id");
        $.post(window.RatesUrls.toggle, { id: id }).done(function () { App.toast("Rate plan updated."); location.reload(); }).fail(App.ajaxError);
    });

    $(document).on("change", ".extra-charge-input", function () {
        var roomTypeId = $(this).data("roomtypeid");
        var row = $(this).closest("tr");
        var extraPersonCharge = row.find('[data-field="extraPersonCharge"]').val();
        var childRate = row.find('[data-field="childRate"]').val();
        $.post(window.RatesUrls.saveExtra, { roomTypeId: roomTypeId, extraPersonCharge: extraPersonCharge, childRate: childRate })
            .done(function () { App.toast("Extra charge updated."); }).fail(App.ajaxError);
    });

    function renderCalendar() {
        var y = calMonth.getFullYear(), m = calMonth.getMonth();
        $("#rcMonthLabel").text(calMonth.toLocaleDateString(undefined, { month: "long", year: "numeric" }));
        var daysInMonth = new Date(y, m + 1, 0).getDate();
        var firstWeekday = new Date(y, m, 1).getDay();
        var roomTypeId = $("#rcRoomType").val();
        var todayIso = new Date().toISOString().slice(0, 10);

        var cells = "";
        for (var i = 0; i < firstWeekday; i++) { cells += '<div class="col" style="width:14.28%;"></div>'; }

        var pending = daysInMonth;
        var results = new Array(daysInMonth + 1);

        for (var d = 1; d <= daysInMonth; d++) {
            (function (day) {
                var dateIso = y + "-" + String(m + 1).padStart(2, "0") + "-" + String(day).padStart(2, "0");
                $.get(window.RatesUrls.getRateForDate, { roomTypeId: roomTypeId, date: dateIso }).done(function (plan) {
                    results[day] = { dateIso: dateIso, plan: plan };
                    pending--;
                    if (pending === 0) { finish(); }
                });
            })(d);
        }

        function finish() {
            var html = cells;
            for (var day = 1; day <= daysInMonth; day++) {
                var r = results[day];
                var isToday = r.dateIso === todayIso;
                html += '<div class="col" style="width:14.28%;"><div class="border rounded p-2 text-center cal-rate-cell' + (isToday ? " border-primary" : "") + '" data-date="' + r.dateIso + '" data-planid="' + (r.plan.planId || "") + '">' +
                    '<div class="text-xs text-muted">' + day + '</div><div class="fw-600 text-xs">' + App.formatCurrency(r.plan.rate) + '</div></div></div>';
            }
            $("#rcGrid").html(html);
        }
    }

    $(document).on("click", ".cal-rate-cell", function () {
        var planId = $(this).data("planid");
        if (planId) {
            $('.edit-rate-btn[data-id="' + planId + '"]').trigger("click");
        } else {
            App.toast("This date uses the room type's base rate. Add a rate plan to override it.", "warning");
        }
    });

    $("#rcRoomType").on("change", renderCalendar);
    $("#rcPrevBtn").on("click", function () { calMonth.setMonth(calMonth.getMonth() - 1); renderCalendar(); });
    $("#rcNextBtn").on("click", function () { calMonth.setMonth(calMonth.getMonth() + 1); renderCalendar(); });

    if (window.flatpickr) {
        flatpickr("#rfStart", { dateFormat: "Y-m-d" });
        flatpickr("#rfEnd", { dateFormat: "Y-m-d" });
    }

    $(function () {
        $("#ratesTable").DataTable({ pageLength: 10, lengthChange: false });
        renderCalendar();
    });
})(jQuery);
