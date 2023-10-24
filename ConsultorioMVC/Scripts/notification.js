$(document).ready(function () {
    $("#btnCloseNotification").on("click", function () {
        $("#notificationContainer").addClass("hidden");
        setTimeout(function () {
            $("#notificationContainer").addClass("d-none");
        }, 2000)
    });
});