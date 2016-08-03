$(document).ready(function () {
    $input.keyup(function () {
        $(this).parent().parent().removeClass("has-error");
    })
});

var $input = $("input");

var $formGr = $(".form-group");

function OnSuccess(response) {
    if (response) {
        window.location.href = "/Home/Index";
    }
    else {
        $formGr.addClass("has-error");
        $("input[name='Username']").focus();
    }
}

function OnFailure() {
    alert("Algo inesperado sucedió! Contacta al administrador");
}