/// <reference path="/Scripts/jquery-1.10.2.min.js" />
/// <reference path="/Scripts/kendo/2014.1.318/jquery-1.9.1.intellisense.js" />
/// <reference path="/Scripts/kendo/2014.1.318/kendo.all.min.js" />


$(document).ready(function () {


    $('#slider_contact .newletterbox_right').click(function () {
        var toggleWidth = $("#slider_contact").width() == 94 ? "320px" : "94px";
        $('#slider_contact').animate({ width: toggleWidth });
    });


    $('.mobile_contact .newletterbox_right').click(function () {
        $(".mobile_contact .newletterbox_left").toggleClass("dn");
    });
    
    

    if ($(window).width() <= 640) {
        $(".header_phne").insertBefore($(".footer"));
        //$(".mobile_contact").show();
    }

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


    $("#mobile_nav").mmenu({
        position: 'left'
    }, {
        // configuration
        offCanvas: {
            pageNodetype: "section"
        }
    });

    // automatic Mikud - start
    $("#Zip").prop('readOnly', true);


    $("#City, #CustomerAddress, #Entrance").focusout(function () {
        var city = $("#City").val();
        var address = $("#CustomerAddress").val();
        var entrance = $("#Entrance").val();

        var text = "";

        if (city != '' && address != '') {

            $.ajax({
                type: "GET",
                url: "/Insurance/_GetMikud",
                data: {
                    city: city,
                    address: address,
                    entrance: entrance,
                },
                dataType: "text",
                success: function (data) {

                    if (data == "NeedEntrance") {
                        $("#Entrance").closest(".row").show();
                        $("#Entrance").closest(".row").find(".field-validation-valid").html("<span class='field-validation-error'>נא להכניס את הכניסה לבניין על מנת למצוא את המיקוד בצורה אוטומטית</span>");
                        $("#Zip").prop('readOnly', false);
                    }
                    else if (data != "CantGetMikud") {

                        $("#Entrance").closest(".row").find(".field-validation-valid").html("");
                        if (entrance == '') {
                            $("#Entrance").closest(".row").hide();
                        }
                        $("#Zip").prop('readOnly', true);
                        $("#Zip").val(data);
                    }

                    else {
                        $("#Zip").prop('readOnly', false);
                    }
                }
            });

        }
    });
    // automatic Mikud - end


    if ($('.insurance_grade input').length) {

        value = $('.insurance_grade input:checked').val();
        if (value == "Female") {
            $('.option_choose_pregnant').show();
        }
        else {
            $('.option_choose_pregnant').hide();
        }

    }

    $(document).on("click", "#print_insureance_button", function () {
        window.print();
    });


    $(".insurance_grade input").change(function () {
        if ($(this).val() == "Female") {
            $('.option_choose_pregnant').show();
        }
        else {
            $('.option_choose_pregnant').hide();
        }
    });

    $("input.submit_ajax_button").removeAttr("disabled");





});