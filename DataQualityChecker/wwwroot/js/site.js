$(document).ready(function () {

    $(".custom-file-input").on("change", function () {
        var fileName = $(this).val().split("\\").pop();
        $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
    });

    var minNumber = 1;
    var maxNumber = 100;
    $('.spinner input').on("change", function () {
        debugger;
        var parsedVal = parseInt($(this).val());
        if (parsedVal > maxNumber) $(this).val("100");
        else if (parsedVal < minNumber) $(this).val("1");
        else $(this).val(parsedVal);
    });

    $('.spinner .btn:first-of-type').on('click', function () {
        if (!$("#noHeaderCheckBox").prop('checked')) {
            if ($('.spinner input').val() == maxNumber) {
                return false;
            } else {
                $('.spinner input').val(parseInt($('.spinner input').val(), 10) + 1);
            }
        }
    });

    $('.spinner .btn:last-of-type').on('click', function () {
        if (!$("#noHeaderCheckBox").prop('checked')) {
            if ($('.spinner input').val() == minNumber) {
                return false;
            } else {
                $('.spinner input').val(parseInt($('.spinner input').val(), 10) - 1);
            }
        }
    });
});
