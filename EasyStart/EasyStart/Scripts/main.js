
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

    $items.each(function () {
        const $item = $(this)

        bindCustomDialogShowEvent($item)
        bindCustomDialogCloseEvent($item)
    })
}

function bindCustomDialogShowEvent($item) {
    const event = () => {
        const animationOption = { effect: "scale", direction: "horizontal" }
       
        $item.addClass('custom-dialog-flex')
        $item.find('.custom-dialog-body').show(animationOption,'', 150)
    }

    $item.bind('showModal', event)
}

function bindCustomDialogCloseEvent($item) {
    const event = () => {
        $item.removeClass('custom-dialog-flex')
        $item.find('.custom-dialog-body').hide()
    }

    $item.bind('close', event)
}

function getCategoryIdByProductIdForPromotion(productId) {
    for (let categoryId in ProductsForPromotion) {
        const products = ProductsForPromotion[categoryId].filter(p => p.Id == productId)

        if (products && products.length > 0)
            return products[0].CategoryId
    }
}