$(document).ready(function () {
    bindPromotinoMenuItemClick()
    StockManger.initStockPeriodCalendar()
})

function bindPromotinoMenuItemClick() {
    $('.promotion-menu li').bind('click', function () { changePromotionActiveMenu(this) })
}

function changePromotionActiveMenu(e) {
    const $e = $(e)
    $('.promotion-menu li').removeClass('promotion-menu-active')
    $e.addClass('promotion-menu-active')

    const targetId = $e.attr('target-id')
    $('.promotion-content').addClass('hide')
    $(`#${targetId}`).removeClass('hide')
}

async function activePromotion() {
    StockManger.initListsProducts()
}