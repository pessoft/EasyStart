$(document).ready(function() {
    $('.promotion-menu li').bind('click', function () { changePromotionActiveMenu(this) })
})

function changePromotionActiveMenu(e) {
    $('.promotion-menu li').removeClass('promotion-menu-active')
    $(e).addClass('promotion-menu-active')
}