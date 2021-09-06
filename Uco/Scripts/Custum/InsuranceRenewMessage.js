$(document).ready(function () {


    $("#CheckboxIDs").val("");

    $(document).on("click", ".ads_select_checkbox_all", function () {
        if ($(this).is(':checked')) {
            $('.ads_select_checkbox').prop('checked', true);
        }
        else {
            $('.ads_select_checkbox').prop('checked', false);
        }
    });

    $(document).on("click", ".ads_select_checkbox_all, .ads_select_checkbox", function () {
        setInterval(cycleAdsCheckbox, 500);
    })


    $(".ads_select_checkbox").each(function (index) {
        if ($(this).is(':checked')) {
            CheckboxInt = CheckboxInt + $(this).attr('data-id') + ",";
        }
    });



    function cycleAdsCheckbox() {
        // load content into video
        var CheckboxInt = "";

        $(".ads_select_checkbox").each(function (index) {
            if ($(this).is(':checked')) {
                CheckboxInt = CheckboxInt + $(this).attr('data-id') + ",";
            }
        });
        $("#CheckboxIDs").val(CheckboxInt);
    }

});