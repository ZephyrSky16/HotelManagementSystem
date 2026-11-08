/* ==========================================================================
   app.js -- Shared shell behavior (sidebar collapse, live clock, toasts,
   confirm dialogs, formatting helpers) used across every Razor view.
   Markup for the sidebar/topbar is now rendered server-side by
   _Sidebar.cshtml / _TopNavigation.cshtml, so this file only owns behavior.
   ========================================================================== */

var App = (function ($) {
    "use strict";

    function initClock() {
        function tick() {
            var now = new Date();
            $("#topbarClockTime").text(now.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit", second: "2-digit" }));
        }
        tick();
        setInterval(tick, 1000);
    }

    function initSidebarToggle() {
        var $shell = $("#appShell");
        var collapsed = localStorage.getItem("revelation_sidebar_collapsed") === "1";
        if (collapsed) { $shell.addClass("sidebar-collapsed"); }

        $("#sidebarToggleBtn").on("click", function () {
            if (window.innerWidth < 992) {
                $shell.toggleClass("sidebar-mobile-open");
            } else {
                $shell.toggleClass("sidebar-collapsed");
                localStorage.setItem("revelation_sidebar_collapsed", $shell.hasClass("sidebar-collapsed") ? "1" : "0");
            }
        });
        $(".sidebar-backdrop").on("click", function () { $shell.removeClass("sidebar-mobile-open"); });
    }

    function initLogout() {
        $("#logoutBtn").on("click", function (e) {
            e.preventDefault();
            if (!window.Swal) { return; }
            Swal.fire({
                title: "Log out?", text: "You will be signed out of the front desk session.", icon: "question",
                showCancelButton: true, confirmButtonText: "Log out", confirmButtonColor: "#1a3a5c"
            }).then(function (result) {
                if (result.isConfirmed) { window.location.href = App.rootUrl + "/"; }
            });
        });
    }

    function toast(message, icon) {
        if (!window.Swal) { return; }
        Swal.mixin({ toast: true, position: "top-end", showConfirmButton: false, timer: 3200, timerProgressBar: true })
            .fire({ icon: icon || "success", title: message });
    }

    function confirmAction(opts) {
        opts = opts || {};
        if (!window.Swal) { return Promise.resolve(true); }
        return Swal.fire({
            title: opts.title || "Are you sure?", text: opts.text || "", html: opts.html,
            icon: opts.icon || "warning", showCancelButton: true,
            confirmButtonText: opts.confirmText || "Confirm", cancelButtonText: opts.cancelText || "Cancel",
            confirmButtonColor: opts.danger ? "#c0392b" : "#1a3a5c", reverseButtons: true
        }).then(function (result) { return result.isConfirmed; });
    }

    function formatCurrency(amount) {
        var n = Number(amount) || 0;
        return "$" + n.toLocaleString("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

    function escapeHtml(str) {
        if (str === null || str === undefined) { return ""; }
        return String(str).replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }

    function ajaxError(xhr) {
        var msg = "Something went wrong. Please try again.";
        if (xhr && xhr.responseJSON && xhr.responseJSON.message) { msg = xhr.responseJSON.message; }
        toast(msg, "error");
    }

    $(function () {
        initClock();
        initSidebarToggle();
        initLogout();
    });

    return {
        toast: toast,
        confirmAction: confirmAction,
        formatCurrency: formatCurrency,
        escapeHtml: escapeHtml,
        ajaxError: ajaxError,
        rootUrl: (window.location.origin)
    };
})(jQuery);
