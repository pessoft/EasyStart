$(document).ready(function () {
    $('.promotion-menu li').bind('click', function () { changePromotionActiveMenu(this) })

    let options = {
        position: "bottom center",
        range: true,
        minDate: new Date(),
        multipleDatesSeparator: " - ",
        toggleSelected: false,
        onHide: function (dp, animationCompleted) {
            if (!dp.maxRange && !animationCompleted) {
                dp.selectDate(dp.minRange);
            }
        }
    };
    let $inputDate = $("#stock-type-calendar-period");
    $inputDate.datepicker(options);
    datePicker = $inputDate.data("datepicker");

    $inputDate.next("i").bind("click", function () {
        datePicker.show();
    })

    const fromDate = new Date()
    const toDate = new Date()
    toDate.setDate(toDate.getDate() + 7)

    datePicker.selectDate([fromDate, toDate]);
})

function changePromotionActiveMenu(e) {
    const $e = $(e)
    $('.promotion-menu li').removeClass('promotion-menu-active')
    $e.addClass('promotion-menu-active')

    const targetId = $e.attr('target-id')
    $('.promotion-content').addClass('hide')
    $(`#${targetId}`).removeClass('hide')
}

const SotckTypePeriod = {
    Unknown: 0,
    OneOff: 1,
    Infinity: 2,
    ToDate: 3
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

const StockConditionTriggerType = {
    Unknown: 0,
    DeliveryOrder: 1,
    SummOrder: 2,
    ProductsOrder: 3
}

const StockConditionDeliveryType = {
    Unknown: 0,
    Delivery: 1,
    TakeYourSelf: 2,
}

var StockManger = {
    addNewStock: function () {
        this.cleanStockDialog()
        this.showStock()
    },
    showStock: function (stockId) {
        const $stockDialog = $('#stockDialog')
        if (stockId) {
            //set stock in stock dialog
        }

        Dialog.showModal($stockDialog)
        $stockDialog.find('select').not('.stock-custom-select').SumoSelect()
    },
    saveStockFromDialog: function () {
    },
    cleanStockDialog: function () {
        this.productsCountConditional = {}

        $('#stockDialog select').not('.stock-custom-select').each(function (i) {
            const sumo = $(this)[0].sumo

            if (sumo)
                sumo.unload()
        })

        $('#stockDialog select option').removeAttr('disabled')
        $('#stockDialog select option').removeAttr('selected')
        $('#stockDialog select option[value=0]').attr('selected', true)
        $('#stockDialog select option[value=0]').attr('disabled', true)
        $('#stockDialog .promotion-stock-index-block.hide-block').hide()
        $('#stockDialog .promotion-stock-dialog-slide').hide()
        $('#stockDialog #stock-slide-1').show()
        $('#stockDialog img').removeAttr('src').addClass('hide')
        $('#stockDialog .dialog-image-upload').removeClass('hide')
        $('#stockDialog textarea').val('')
        $('#stockDialog input[type=text]').val('')
        $('#stockDialog input[type = file]').val('')
        $('#stock-discount-val').val('')
        $('#stock-products-count').val(1)
        $('#bonus-product-items')[0].sumo.unSelectAll()
        $('#discount-type').removeAttr('selected').find('option[value=1]').attr('selected', true)
        $('#stock-condition-sum-count').val('')
        $('#condition-product-items')[0].sumo.unSelectAll()


        $(".promotion-stock-next").attr('disabled', true)

        datePicker = $("#stock-type-calendar-period").data("datepicker");
        const fromDate = new Date()
        const toDate = new Date()
        toDate.setDate(toDate.getDate() + 7)

        datePicker.selectDate([fromDate, toDate]);
    },
    nextStockSlide: function (eIdShow, eIdHide) {
        $(`#${eIdHide}`).hide("slide", { direction: "left" }, 100, () => {
            $(`#${eIdShow}`).show("slide", { direction: "right" }, 100)
        });
    },
    prevStockSlide: function (eIdShow, eIdHide) {
        $(`#${eIdHide}`).hide("slide", { direction: "right" }, 100, () => {
            $(`#${eIdShow}`).show("slide", { direction: "left" }, 100)
        });
    },
    slide: {
        StockType: 1,
        RewardType: 2,
        ConditionType: 3,
        GeneralDescriptionType: 4
    },
    onStockTypePeriodChange: function () {
        const animationOption = { effect: "scale", direction: "horizontal" }
        const stockType = parseInt($('#stock-type-period option:selected').val())
        const callback = () => {
            switch (stockType) {
                case SotckTypePeriod.OneOff:
                    $('#stock-one-type-subtype-container').show(animationOption, '', 150)
                    break
                case SotckTypePeriod.ToDate:
                    $('#stock-type-calendar-container').show(animationOption, '', 150)
                    break
            }
        }

        const $hideBlock = $('#stock-slide-1 .promotion-stock-index-block.hide-block:visible')
        if (stockType != SotckTypePeriod.Unknown && $hideBlock.length > 0)
            $hideBlock.each(function () {
                $(this).hide(
                    animationOption,
                    '',
                    150,
                    callback)
            })
        else if ($hideBlock.length == 0)
            callback()

        this.btnNextToggle(this.slide.StockType)
    },
    btnNextToggle: function (slide) {
        const self = this

        const stockTypeToggle = () => {
            const stockType = parseInt($('#stock-type-period option:selected').val())

            switch (stockType) {
                case SotckTypePeriod.OneOff:
                    const valOneType = $('#stock-one-type-subtype').val()

                    if (valOneType)
                        $("#stock-slide-1 .promotion-stock-next").removeAttr('disabled')
                    else
                        $("#stock-slide-1 .promotion-stock-next").attr('disabled', true)
                    break
                case SotckTypePeriod.Infinity:
                case SotckTypePeriod.ToDate:
                    $("#stock-slide-1 .promotion-stock-next").removeAttr('disabled')
                    break
            }
        }

        const rewardTypeToggle = () => {
            const rewardType = parseInt($('#stock-type-reward option:selected').val())

            switch (rewardType) {
                case RewardType.Discout:
                    const val = parseInt($('#stock-discount-val').val())

                    if (Number.isNaN(val) || val == 0)
                        $("#stock-slide-2 .promotion-stock-next").attr('disabled', true)
                    else
                        $("#stock-slide-2 .promotion-stock-next").removeAttr('disabled')

                    break
                case RewardType.Products:
                    const allowedCountProducts = parseInt($('#stock-products-count').val())
                    const countSelectedProducts = $('#bonus-product-items option:selected').length

                    if (Number.isNaN(allowedCountProducts)
                        || allowedCountProducts < 1
                        || countSelectedProducts == 0)
                        $("#stock-slide-2 .promotion-stock-next").attr('disabled', true)
                    else
                        $("#stock-slide-2 .promotion-stock-next").removeAttr('disabled')
                    break
            }
        }

        const conditionTypeToggle = () => {
            const conditionType = parseInt($('#stock-condition-type option:selected').val())
            const disabledNextAction = () => $("#stock-slide-3 .promotion-stock-next").attr('disabled', true)
            const enabledNextAction = () => $("#stock-slide-3 .promotion-stock-next").removeAttr('disabled')

            switch (conditionType) {
                case StockConditionTriggerType.DeliveryOrder:
                    const deliveryType = parseInt($('#stock-condition-delivery-type option:selected').val())

                    if (deliveryType == StockConditionDeliveryType.Unknown
                        || Number.isNaN(deliveryType))
                        disabledNextAction()
                    else
                        enabledNextAction()
                    break
                case StockConditionTriggerType.SummOrder:
                    const minSum = parseInt($('#stock-condition-sum-count').val())

                    if (!Number.isNaN(minSum) && minSum > 0)
                        enabledNextAction()
                    else
                        disabledNextAction()
                    break
                case StockConditionTriggerType.ProductsOrder:
                    if (self.productsCountConditional
                        && Object.keys(self.productsCountConditional).length > 0) {
                        for (let key in self.productsCountConditional) {
                            const val = parseInt(self.productsCountConditional[key].count)

                            if (Number.isNaN(val) || val < 1) {
                                disabledNextAction()
                                return
                            }
                        }

                        enabledNextAction()
                    } else
                        disabledNextAction()
                    break
            }
        }

        const generalDescriptionType = () => {
            if ($('#promotion-stock-name').val().trim()
                && $('#promotion-stock-description').val().trim())
                $("#stock-slide-4 .promotion-stock-next").removeAttr('disabled')
            else
                $("#stock-slide-4 .promotion-stock-next").attr('disabled', true)
        }

        switch (slide) {
            case this.slide.StockType:
                stockTypeToggle()
                break
            case this.slide.RewardType:
                rewardTypeToggle()
                break
            case this.slide.ConditionType:
                conditionTypeToggle()
                break
            case this.slide.GeneralDescriptionType:
                generalDescriptionType()
                break

        }
    },
    onRewardChangeType: function () {
        const animationOption = { effect: "scale", direction: "horizontal" }
        const rewardType = parseInt($('#stock-type-reward option:selected').val())
        const callback = () => {
            switch (rewardType) {
                case RewardType.Discout:
                    $('#stock-type-discount-container').show(animationOption, '', 150)
                    break
                case RewardType.Products:
                    $('#stock-type-products-container').show(animationOption, '', 150)
                    break
            }
        }

        const $hideBlock = $('#stock-slide-2 .promotion-stock-index-block.hide-block:visible')
        if (rewardType != RewardType.Unknown && $hideBlock.length > 0)
            $hideBlock.each(function () {
                $(this).hide(
                    animationOption,
                    '',
                    150,
                    callback)
            })
        else if ($hideBlock.length == 0)
            callback()

        this.btnNextToggle(this.slide.RewardType)
    },
    onDicountChange: function () {
        const discountType = parseInt($('#discount-type option:selected').val())
        let val = parseInt($('#stock-discount-val').val())

        if (Number.isNaN(val))
            val = ''
        else if (discountType == DiscountType.Percent) {
            if (val < 0)
                val = 0
            else if (val > 100)
                val = 100
        }
        else if (val < 0)
            val = 0

        $('#stock-discount-val').val(val)

        this.btnNextToggle(this.slide.RewardType)
    },
    onDiscountTypeChange: function () {
        this.onDicountChange()
    },
    onBonusProductsChange: function () {
        this.btnNextToggle(this.slide.RewardType)
    },
    onAllowedBonusCountChange: function () {
        this.btnNextToggle(this.slide.RewardType)
    },
    onTriggerConditionChangeType: function () {
        const animationOption = { effect: "scale", direction: "horizontal" }
        const conditionType = parseInt($('#stock-condition-type option:selected').val())
        const callback = () => {
            switch (conditionType) {
                case StockConditionTriggerType.DeliveryOrder:
                    $('#stock-condition-delivery-container').show(animationOption, '', 150)
                    break
                case StockConditionTriggerType.SummOrder:
                    $('#stock-condition-summ-container').show(animationOption, '', 150)
                    break
                case StockConditionTriggerType.ProductsOrder:
                    $('#stock-condition-products-container').show(
                        animationOption,
                        '',
                        150,
                        this.onStockConditionProductsChange())

                    break
            }
        }

        const $hideBlock = $('#stock-slide-3 .promotion-stock-index-block.hide-block:visible')
        if (conditionType != StockConditionTriggerType.Unknown && $hideBlock.length > 0)
            $hideBlock.each(function () {
                $(this).hide(
                    animationOption,
                    '',
                    150,
                    callback)
            })
        else if ($hideBlock.length == 0)
            callback()

        this.btnNextToggle(this.slide.ConditionType)
    },
    onStockConditionDeliveryTypeChange: function () {
        this.btnNextToggle(this.slide.ConditionType)
    },
    onStockConditionSumChange: function () {
        this.btnNextToggle(this.slide.ConditionType)
    },
    productsCountConditional: {},
    onStockConditionProductsChange: function () {
        const animationOption = { effect: "scale", direction: "horizontal" }
        const $products = $('#condition-product-items option:selected')
        const idProductsCount = 'stock-condition-products-count-container'
        const self = this
        if ($products.length > 0) {
            const tmpProducts = {}
            $products.each(function () {
                const $self = $(this)
                const productId = $self.val()
                const categoryId = $self.attr('category-id')
                const prevValue = self.productsCountConditional[productId]

                tmpProducts[productId] = {
                    categoryId: categoryId,
                    count: prevValue ? prevValue.count : 1
                }
            })

            this.productsCountConditional = tmpProducts
            const countItems = []
            for (let productId in this.productsCountConditional) {
                const data = this.productsCountConditional[productId]
                const product = ProductsForPromotion[data.categoryId].filter(p => p.Id == productId)[0]

                const span = `<span title="${product.Name}">${product.Name}</span>`
                const iNumber = `<input 
                                    onfocusout="StockManger.onStockConditionProductsFocusOut(${productId}, this)"
                                    onchange="StockManger.onStockConditionProductCountChange(${productId}, this)"
                                    type="number"
                                    min="1"
                                    value="${data.count}">`
                const row = `<div class="stock-product-count-item">${span}${iNumber}</div>`

                countItems.push(row)
            }

            $(`#${idProductsCount} .stock-setting-condition-count-products`).empty()
            $(`#${idProductsCount} .stock-setting-condition-count-products`).html(countItems)
            $(`#${idProductsCount}`).show(animationOption, '', 150)
        }
        else {
            this.productsCountConditional = {}
            $(`#${idProductsCount}`).hide(animationOption, '', 150)
        }

        this.btnNextToggle(this.slide.ConditionType)
    },
    onStockConditionProductsFocusOut: function (productId, e) {
        const $e = $(e)
        const val = parseInt($e.val())

        if (Number.isNaN(val) || val < 1) {
            const data = this.productsCountConditional[productId]

            if (data.count < 1)
                data.count = 1

            $e.val(data.count)
        }
    },
    onStockConditionProductCountChange: function (productId, e) {
        const data = this.productsCountConditional[productId]
        const count = parseInt($(e).val())
        if (data && !Number.isNaN(count))
            data.count = count
    },
    onGeniralDescriptionChange: function () {
        this.btnNextToggle(this.slide.GeneralDescriptionType)
    },
    toggleSave: function (e) {
        const $e = $(e)

        if ($e.attr('src'))
            $("#stock-slide-5 .promotion-stock-next").removeAttr('disabled')
        else 
            $("#stock-slide-5 .promotion-stock-next").attr('disabled', true)
    }
}

async function activePromotion() {
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

    if (ProductsForPromotion) {
        const $newSelect = $(`<select id="${idSelect}" onchange="StockManger.onBonusProductsChange()" class="stock-custom-select" multiple placeholder="Выберите блюда"></select>`)
        const $newSelectConditino = $(`<select id="${idSelectCondition}" onchange="StockManger.onStockConditionProductsChange()" class="stock-custom-select" multiple placeholder="Выберите блюда"></select>`)
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

        const $select = $(`#${idSelect}`)
        const $selectCondition = $(`#${idSelectCondition}`)
        const sumoOptions = {
            search: true,
            searchText: 'Поиск...',
            noMatch: 'Нет совпадений для "{0}"',
            captionFormat: 'Выбрано блюд: {0}',
            captionFormatAllSelected: 'Выбраны все блюда: {0}'
        }

        $select.SumoSelect(sumoOptions)
        $selectCondition.SumoSelect(sumoOptions)
    }
}

