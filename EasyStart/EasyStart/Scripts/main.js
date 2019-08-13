
function successCallBack(func, loader) {
    return function (data) {
        if (func) {
            func(data, loader);
        }
    }
}

function generateRandomString() {
    return Math.random().toString(36).slice(-6);
}


function showInfoMessage(message) {
    $.notify(message, {
        position: "top left",
        className: "info"
    });
}

function showWarningMessage(message) {
    $.notify(message, {
        position: "top left",
        className: "warn"
    });
}

function showErrorMessage(message) {
    $.notify(message, {
        position: "top left",
        className: "error"
    });
}

function xFormatPrice(_number) {
    var decimal = 0;
    var separator = ' ';
    var decpoint = '.';
    var format_string = '# ';

    if (!isInteger(_number)) {
        decimal = 2;
    }

    var r = parseFloat(_number)

    var exp10 = Math.pow(10, decimal);// приводим к правильному множителю
    r = Math.round(r * exp10) / exp10;// округляем до необходимого числа знаков после запятой

    rr = Number(r).toFixed(decimal).toString().split('.');

    b = rr[0].replace(/(\d{1,3}(?=(\d{3})+(?:\.\d|\b)))/g, "\$1" + separator);

    r = (rr[1] ? b + decpoint + rr[1] : b);
    return format_string.replace('#', r);
}