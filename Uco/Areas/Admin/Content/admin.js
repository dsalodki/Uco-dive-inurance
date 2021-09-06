function endsWith(str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

function startWith(str, suffix) {
    return str.indexOf(suffix, 0) !== -1;
}

$(document).ready(function () {

    $(document).on("click", ".insurance_edit_delete input", function () {
        if (confirm("האם אתה בטוח שברצונך למחוק?")) {
        }
        else {
            return false;
        }
    });

    $(document).on("click", ".open_insurance_send_popup", function () {
        window.open($(this).attr("href"), "mywindow", "location=1,status=1,scrollbars=1, width=400,height=200");

        return false;
    });

    $("form").kendoValidator();
    $("#content").css("height", ($(window).height() - 28) + "px");
    $("#indexViewIframe").attr("height", $("#indexViewDiv").height() + "px");
});

window.onresize = function (event) {
    $("#content").css("height", ($(window).height() - 28) + "px");
    $("#indexViewIframe").attr("height", $("#indexViewDiv").height() + "px");
}

function changeViewFrame(newSrc) {
    $("#indexViewIframe").attr("src", newSrc);
}