$(document).ready(function () {
    bindPromotinoMenuItemClick()

    StockManger.initPeriodCalendar()
    CouponManager.initPeriodCalendar()

    PromotionSetting.bindDragula()
    PromotionSetting.binSumoSelectForPromotionSettings()
    PromotionSetting.loadSettings()
    
})

const PromotionSection = {
    Unknown: 0,
    Stock: 2,
    Coupon: 4,
    Partners: 8
}

const RewardType = {
    Unknown: 0,
    Discout: 1,
    Products: 2
}

const DiscountType = {
    Unknown: 0,
    Percent: 1,
    Ruble: 2
}

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
    initListsProducts()
}

function initListsProducts () {
    const idSelect = 'bonus-product-items'
    const $contentWrapper = $('#bonus-products-setting')
    const $select = $contentWrapper.find(`#${idSelect}`)

    if ($select.length > 0 && $select[0].sumo)
        $select[0].sumo.unload()

    $select.remove()

    const idSelectCondition = 'condition-product-items'
    const $contentConditionWrapper = $('#stock-condition-products-container')
    const $selectCondition = $contentConditionWrapper.find(`#${idSelectCondition}`)

    if ($selectCondition.length > 0 && $selectCondition[0].sumo)
        $selectCondition[0].sumo.unload()

    $selectCondition.remove()

    const idSelectCoupon = 'coupon-bonus-products-items'
    const $contentCouponWrapper = $('#coupon-bonus-products-setting')
    const $selectCoupon = $contentCouponWrapper.find(`#${idSelectCoupon}`)

    if ($selectCoupon.length > 0 && $selectCoupon[0].sumo)
        $selectCoupon[0].sumo.unload()

    $selectCoupon.remove()

    if (ProductsForPromotion) {
        const $newSelect = $(`<select id="${idSelect}" onchange="StockManger.onBonusProductsChange()" class="promotion-custom-select" multiple placeholder="Выберите блюда"></select>`)
        const $newSelectConditino = $(`<select id="${idSelectCondition}" onchange="StockManger.onStockConditionProductsChange()" class="promotion-custom-select" multiple placeholder="Выберите блюда"></select>`)
        const $newSelectCoupon = $(`<select id="${idSelectCoupon}" onchange="CouponManager.onBonusProductsChange()" class="promotion-custom-select" multiple placeholder="Выберите блюда"></select>`)
        const selectContent = []

        for (let categoryId in ProductsForPromotion) {
            const categoryName = CategoryDictionary[categoryId]
            const products = ProductsForPromotion[categoryId]

            let options = ''
            for (let product of products) {
                options += `<option value='${product.Id}' category-id='${product.CategoryId}'>${product.Name}</option>`
            }
            const optgroup = `<optgroup label='${categoryName}'>${options}</optgroup>`

            selectContent.push(optgroup)
        }

        $newSelect.append(selectContent)
        $contentWrapper.append($newSelect)
        $newSelectConditino.append(selectContent)
        $contentConditionWrapper.append($newSelectConditino)
        $newSelectCoupon.append(selectContent)
        $contentCouponWrapper.append($newSelectCoupon)

        const $select = $(`#${idSelect}`)
        const $selectCondition = $(`#${idSelectCondition}`)
        const $selectCoupon = $(`#${idSelectCoupon}`)
        const sumoOptions = {
            search: true,
            searchText: 'Поиск...',
            noMatch: 'Нет совпадений для "{0}"',
            captionFormat: 'Выбрано блюд: {0}',
            captionFormatAllSelected: 'Выбраны все блюда: {0}'
        }

        $select.SumoSelect(sumoOptions)
        $selectCondition.SumoSelect(sumoOptions)
        $selectCoupon.SumoSelect(sumoOptions)
    }
}
