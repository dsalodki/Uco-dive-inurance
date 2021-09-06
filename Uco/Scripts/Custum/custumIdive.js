/// <reference path="/Scripts/jquery-1.10.2.min.js" />
/// <reference path="/Scripts/kendo/2014.1.318/jquery-1.9.1.intellisense.js" />
/// <reference path="/Scripts/kendo/2014.1.318/kendo.all.min.js" />

/***** Mobile: isMobile.any() *****/
var isMobile = { Android: function () { return navigator.userAgent.match(/Android/i) ? !0 : !1 }, BlackBerry: function () { return navigator.userAgent.match(/BlackBerry/i) ? !0 : !1 }, iOS: function () { return navigator.userAgent.match(/iPhone|iPad|iPod/i) ? !0 : !1 }, Opera: function () { return navigator.userAgent.match(/Opera Mini/i) ? !0 : !1 }, Windows: function () { return navigator.userAgent.match(/IEMobile/i) ? !0 : !1 }, any: function () { return isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows() } };

$(document).ready(function () {

    $(".right_sec_text1_part").click(function () {
        if (window.matchMedia("(max-width: 980px)").matches) {
            $(".right_sec_text1_part > table").toggle();
        }
    });

    //datepicker
    $(".datePicker").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'dd-mm-yyyy'
    });


    //menu
    jQuery(document).ready(function () {
        jQuery('ul.sf-menu').superfish({
            autoArrows: false,
            cssArrows: false,
        });
    });

    //mobile menu
    $("#my-menu").mmenu({
        extensions: ["theme-dark"],
        offCanvas: {
            position: "right",
            zposition: "front",
            pageNodetype: "section"
        },
        navbar: {
            title: "תפריט"
        }
    });

    $('a[target = "_blank"]').click(function (event) {
        event.preventDefault();
        var width = $(window).width();
        if (width > 1400) {
            width = width / 1.4;

        }
        else {
            width = width / 1.1;
        }
        var height = $(window).height() / 1.2;
        window.open($(this).attr("href"), "popupWindow", "width=" + width + ",height=" + height + ",scrollbars=yes");
    });

    $('#slider_contact .newletterbox_right').click(function () {
        var toggleWidth = $("#slider_contact").width() == 80 ? "305px" : "80px";
        $('#slider_contact').animate({ width: toggleWidth });
    });


    $('.mobile_contact .newletterbox_right').click(function () {
        $(".mobile_contact .newletterbox_left").toggleClass("dn");
    });

    $(".addVote").on("click", function (e) {
        var photographerID = $(this).data("photographerid");
        $.ajax({
            type: "Get",
            dataType: "Json",
            cache: false,
            url: "/Competition/AddVote",
            data: { photographerID: photographerID }
        }).done(function (data) {
            if (data.status == "ok") {
                location.reload();
            }
            if (data.status == "voted") {
                alert(data.message);
            }
            if (data.status == "login") {
                window.location.href = data.redirect;
            }
            if (data.status == "error") {
                alert("שגיאה");
            }
        });
        e.preventDefault();
    });
    if (isMobile.any()) {
        $('.artile_text2').each(function () {
            $(this).html($(this).html().replace(/&nbsp;/gi, ''));
        });
        $('#content .editor_html').each(function () {
            $(this).html($(this).html().replace(/&nbsp;/gi, ''));
        });
    }

    if ($(window).width() <= 800) {
        $(".right_sec_form").insertAfter(".left_sec");
    }

    $(window).resize(function () {
        if ($(window).width() <= 800) {
            $(".right_sec_form").insertAfter(".left_sec");
        } else {
            $(".right_sec_form").insertBefore(".right_green_btn1");
        }
    });
});