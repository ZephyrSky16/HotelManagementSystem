/* ==========================================================================
   rooms.js -- Room Management: add/edit, activate/deactivate, details.
   ========================================================================== */

(function ($) {
    "use strict";

    function initTable() {
        $("#roomsTable").DataTable({ pageLength: 10, lengthChange: false, order: [[0, "asc"]] });
        $("#roomsSearch").on("input", function () { $("#roomsTable").DataTable().search($(this).val()).draw(); });
    }

    function resetForm() {
        $("#roomForm")[0].reset();
        $("#rfId").val("");
        $("#rfActive").prop("checked", true);
    }

    $("#addRoomBtn").on("click", function () {
        resetForm();
        $("#roomFormTitle").text("Add Room");
        new bootstrap.Modal(document.getElementById("roomFormModal")).show();
    });

    $(document).on("click", ".edit-room-btn", function (e) {
        e.preventDefault();
        var row = $(this).closest("tr");
        resetForm();
        $("#rfId").val($(this).data("id"));
        $("#roomFormTitle").text("Edit Room " + row.find("td:eq(0)").text().trim());
        $("#rfNumber").val(row.find("td:eq(0)").text().trim());
        $("#rfName").val(row.find("td:eq(1)").text().trim() === "--" ? "" : row.find("td:eq(1)").text().trim());
        $("#rfFloor").val(row.find("td:eq(3)").text().trim());
        $("#rfCapacity").val(row.find("td:eq(4)").text().trim());
        $("#rfBed").val(row.find("td:eq(5)").text().trim());
        $("#rfRate").val(row.find("td:eq(8)").text().trim().replace(/[^0-9.]/g, ""));
        $("#rfActive").prop("checked", row.find("td:eq(9)").text().trim() === "Active");

        var typeText = row.find("td:eq(2)").text().trim();
        $("#rfType option").each(function () { if ($(this).text() === typeText) { $(this).prop("selected", true); } });

        var statusText = row.find("td:eq(6)").text().trim().replace(/\s/g, "");
        $("#rfStatus").val(statusText);

        new bootstrap.Modal(document.getElementById("roomFormModal")).show();
    });

    $("#roomForm").on("submit", function (e) {
        e.preventDefault();
        var data = {
            id: $("#rfId").val() || null, number: $("#rfNumber").val().trim(), name: $("#rfName").val().trim(),
            roomTypeId: $("#rfType").val(), floor: $("#rfFloor").val(), capacity: $("#rfCapacity").val(),
            bedConfiguration: $("#rfBed").val(), baseRate: $("#rfRate").val(), status: $("#rfStatus").val(),
            active: $("#rfActive").is(":checked")
        };
        $.post(window.RoomsUrls.saveRoom, data).done(function (r) {
            if (!r.success) { App.toast(r.message, "error"); return; }
            App.toast("Room saved.");
            bootstrap.Modal.getInstance(document.getElementById("roomFormModal")).hide();
            location.reload();
        }).fail(App.ajaxError);
    });

    $(document).on("click", ".toggle-active-btn", function (e) {
        e.preventDefault();
        var id = $(this).data("id");
        App.confirmAction({ title: "Change this room's active status?", confirmText: "Confirm" }).then(function (ok) {
            if (!ok) { return; }
            $.post(window.RoomsUrls.toggleActive, { id: id }).done(function () {
                App.toast("Room status updated.");
                location.reload();
            }).fail(App.ajaxError);
        });
    });

    $(document).on("click", ".view-room-btn", function (e) {
        e.preventDefault();
        var id = $(this).data("id");
        $("#roomDetailsBody").html("Loading...");
        new bootstrap.Modal(document.getElementById("roomDetailsModal")).show();
        $.get(window.RoomsUrls.getRoomDetails, { id: id }).done(function (html) { $("#roomDetailsBody").html(html); }).fail(App.ajaxError);
    });

    $(function () { initTable(); });
})(jQuery);
