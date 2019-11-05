
function successCallBack(func, loader) {
    return function (data) {
        if (func) {
            func(data, loader);
        }
    }
}

function generateRandomString(length = 6) {
    return Math.random().toString(36).slice(-length);
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

var ProductType = {
    Normal: 0,
    New: 1,
    Hit: 2,
    Stock: 4,
    HotPeppers: 8,
    Vegetarion: 16
}

class BitOperation {
    static Add(item1, item2) {
        return item1 | item2
    }

    static isHas(item, itemCheck) {
        return (item & itemCheck) == itemCheck
    }
}

function bindCustomDialogToggleEvent(id) {
    const $items = id ? $(`#${id}`) : $('.custom-dialog')

    bindCustomDialogShowEvent($items)
    bindCustomDialogCloseEvent($items)
}

function bindCustomDialogShowEvent($items) {
    const event = () => {
        const animationOption = { effect: "scale", direction: "horizontal" }
       
        $items.addClass('custom-dialog-flex')
        $items.find('.custom-dialog-body').show(animationOption,'', 150)
    }

    $items.bind('showModal', event)
}

function bindCustomDialogCloseEvent($items) {
    const event = () => {
        $items.removeClass('custom-dialog-flex')
        $items.find('.custom-dialog-body').hide()
    }

    $items.bind('close', event)
}