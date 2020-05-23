const SotckTypePeriod = {
    Unknown: 0,
    OneOff: 1,
    Infinity: 2,
    ToDate: 3
}

const StockOneTypeSubtype = {
    Unknown: 0,
    FirstOrder: 1,
    OneOrder: 2
}

const StockConditionTriggerType = {
    Unknown: 0,
    DeliveryOrder: 1,
    SummOrder: 2,
    ProductsOrder: 3,
    HappyBirthday: 4
}

const StockConditionDeliveryType = {
    Unknown: 0,
    TakeYourSelf: 1,
    Delivery: 2,
}

var StockManger = {
    stockList: [],
    initPeriodCalendar: function () {
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
            $inputDate.focus();
        })

        const fromDate = new Date()
        const toDate = new Date()
        toDate.setDate(toDate.getDate() + 7)

        datePicker.selectDate([fromDate, toDate]);
    },
    loadStockList: function () {
        const self = this

        this.clearStockList();
        if (this.stockList &&
            this.stockList.length > 0) {
            this.addAllItemStock(this.stockList);
        } else {
            let loader = new Loader($("#stock-list"));
            let successFunc = function (result, loader) {
                loader.stop();
                if (result.Success) {
                    if (!result.Data || result.Data.length == 0) {
                        self.setEmptyStockInfo();
                    } else {
                        result.Data.forEach(p => self.processingLoadStockData(p))
                        self.stockList = result.Data;
                        self.addAllItemStock(self.stockList);
                    }
                } else {
                    showErrorMessage(result.ErrorMessage);
                    self.setEmptyStockInfo();
                }
            }
            loader.start();

            $.post("/Admin/LoadStockList", null, successCallBack(successFunc, loader));
        }
    },
    setEmptyStockInfo: function () {
        let template = `
        <div class="empty-list">
            <i class="fal fa-smile-plus"></i>
            <span>Добавте акции</span>
        </div>`;

        $("#stock-list").append(template);
    },
    addAllItemStock: function (data) {
        for (let stock of data) {
            this.addStockToList(stock);
        }
    },
    getStockTileTemplate: function (stock) {
        let $template = $($("#stock-item-template").html());
        const self = this

        $template.attr("stock-id", stock.Id);
        $template.find("img").attr("src", stock.Image);
        $template.find(".stock-item-name").html(stock.Name);
        $template.find(".stock-remove").bind("click", function () {
            let callback = () => self.removeStock(stock.Id);

            deleteConfirmation(callback);
        });
        $template.find(".stock-edit").bind("click", function () {
            self.editStock(stock.Id);
        });

        return $template
    },
    replaceStockInList: function (stock, replaceStockId) {
        let $template = this.getStockTileTemplate(stock)

        $(`#stock-list [stock-id="${replaceStockId}"]`).replaceWith($template);
    },
    addStockToList: function (stock) {
        let $template = this.getStockTileTemplate(stock)

        $("#stock-list").append($template);
    },
    addNewStock: function () {
        this.cleanStockDialog()
        this.showStock()
    },
    showStock: function (stockId) {
        const $stockDialog = $('#stockDialog')

        $stockDialog.find('select').not('.promotion-custom-select').each(function (i) {
            const sumo = $(this)[0].sumo

            if (!sumo)
                $(this).SumoSelect()
        })

        if (stockId) {
            this.setStockData(stockId)
        }

        Dialog.showModal($stockDialog)

    },
    setStockData: function (stockId) {
        const data = this.getStockById(stockId)

        $('#stockDialog').attr('stock-id', data.Id)

        this.setSockTypePeriod(data)
        this.setStockReward(data)
        this.setStockCondition(data)
        this.setStockGeneralDescription(data)
        this.setStockImage(data)

        $(".promotion-stock-next").removeAttr('disabled')
    },
    setSockTypePeriod: function (data) {
        $(`#stock-type-period`)[0].sumo.selectItem(data.StockTypePeriod)

        switch (data.StockTypePeriod) {
            case SotckTypePeriod.OneOff:
                $('#stock-one-type-subtype-container').show()
                $(`#stock-one-type-subtype`)[0].sumo.selectItem(data.StockOneTypeSubtype)
                break;
            case SotckTypePeriod.ToDate:
                $('#stock-type-calendar-container').show()

                let datePicker = $("#stock-type-calendar-period").data("datepicker");
                datePicker.selectDate([data.StockFromDate, data.StockToDate]);
                break;
        }
    },
    setStockReward: function (data) {
        $(`#stock-type-reward`)[0].sumo.selectItem(data.RewardType)

        switch (data.RewardType) {
            case RewardType.Discount:
                $('#stock-type-discount-container').show()

                $('#stock-discount-val').val(data.DiscountValue)
                $(`#discount-type`)[0].sumo.selectItem(data.DiscountType.toString())

                $('#stock-products-count').val(data.CountBonusProducts)
                break;
            case RewardType.Products:
                $('#stock-type-products-container').show()

                $('#stock-products-count').val(data.CountBonusProducts)
                data.AllowedBonusProducts.forEach(p => {
                    $(`#bonus-product-items`)[0].sumo.selectItem(p.toString())
                })
                break;
        }
    },
    setStockCondition: function (data) {
        $(`#stock-condition-type`)[0].sumo.selectItem(data.ConditionType)

        switch (data.ConditionType) {
            case StockConditionTriggerType.DeliveryOrder:
                $('#stock-condition-delivery-container').show()
                $(`#stock-condition-delivery-type`)[0].sumo.selectItem(data.ConditionDeliveryType)
                break;
            case StockConditionTriggerType.SummOrder:
                $('#stock-condition-summ-container').show()
                $('#stock-condition-sum-count').val(data.ConditionOrderSum)

                if (data.StockExcludedProducts) {
                    data.StockExcludedProducts.forEach(p => {
                        $(`#stock-excluded-products-items`)[0].sumo.selectItem(p.toString())
                    })
                }
                break;
            case StockConditionTriggerType.ProductsOrder:
                $('#stock-condition-products-container').show()
                $('#stock-condition-products-count-container').show()

                for (let productId in data.ConditionCountProducts) {
                    $(`#condition-product-items`)[0].sumo.selectItem(productId.toString())

                    this.productsCountConditional[productId] = {
                        categoryId: getCategoryIdByProductIdForPromotion(productId),
                        count: data.ConditionCountProducts[productId]
                    }
                }

                const countItems = this.getProductCountConditionalItems()
                $(`#stock-condition-products-count-container .stock-setting-condition-count-products`).html(countItems)

                break;
            case StockConditionTriggerType.HappyBirthday:
                $('#stock-condition-birthday-container').show()
                $('#stock-condition-birthday-period-before').val(data.ConditionBirthdayBefore)
                $('#stock-condition-birthday-period-after').val(data.ConditionBirthdayAfter)
                break;
        }
    },
    setStockGeneralDescription: function (data) {
        $('#promotion-stock-name').val(data.Name)
        $('#promotion-stock-description').val(data.Description)
    },
    setStockImage: function (data) {
        const $img = $('#stockDialog img')
        $img.attr("src", data.Image);
        $img.removeClass("hide");

        $('#stockDialog .dialog-image-upload').addClass('hide')
    },
    saveStockFromDialog: function () {
        const self = this
        const stockIdToRemove = parseInt($('#stockDialog').attr('stock-id'))
        let loader = new Loader($("#stockDialog .custom-dialog-body"));
        loader.start();

        const getAllowedBonusProductsJSON = () => {
            const prodictIds = []
            $('#bonus-product-items option:selected').each(function () {
                prodictIds.push(parseInt($(this).val()))
            })

            return JSON.stringify(prodictIds)
        }

        const getStockExcludedProductsJSON = () => {
            const prodictIds = []
            $('#stock-excluded-products-items option:selected').each(function () {
                prodictIds.push(parseInt($(this).val()))
            })

            return JSON.stringify(prodictIds)
        }

        const getConditionCountProductsJSON = () => {
            const dict = {} //key - productId, value - count

            $('#stock-condition-products-count-container .stock-product-count-item').each(function () {
                const $self = $(this)
                const id = $self.find('span').attr('product-id')
                const count = parseInt($self.find('input').val())

                dict[id] = count
            })

            return JSON.stringify(dict);
        }

        const getIntValue = str => {
            let value = parseInt(str)

            if (Number.isNaN(value))
                value = 0

            return value
        }

        const stock = {
            id: $('#stockDialog').attr('stock-id'),
            stockTypePeriod: parseInt($('#stock-type-period option:selected').val()),
            stockOneTypeSubtype: parseInt($('#stock-one-type-subtype option:selected').val()),
            stockFromDate: $("#stock-type-calendar-period").data("datepicker").selectedDates[0].toJSON(),
            stockToDate: $("#stock-type-calendar-period").data("datepicker").selectedDates[1].toJSON(),
            rewardType: parseInt($('#stock-type-reward option:selected').val()),
            discountValue: getIntValue($('#stock-discount-val').val()),
            discountType: parseInt($('#discount-type option:selected').val()),
            countBonusProducts: parseInt($('#stock-products-count').val()),
            allowedBonusProductsJSON: getAllowedBonusProductsJSON(),
            stockExcludedProductsJSON: getStockExcludedProductsJSON(),
            conditionType: parseInt($('#stock-condition-type option:selected').val()),
            conditionDeliveryType: parseInt($('#stock-condition-delivery-type option:selected').val()),
            conditionOrderSum: getIntValue($('#stock-condition-sum-count').val()),
            conditionCountProductsJSON: getConditionCountProductsJSON(),
            conditionBirthdayBefore: getIntValue($('#stock-condition-birthday-period-before').val()),
            conditionBirthdayAfter: getIntValue($('#stock-condition-birthday-period-after').val()),
            name: $('#promotion-stock-name').val(),
            description: $('#promotion-stock-description').val(),
            image: $("#stockDialog img").attr("src")
        }

        const successFunc = function (result, loader) {
            loader.stop();
            if (result.Success) {
                $("#stock-list .empty-list").remove();
                self.processingLoadStockData(result.Data)

                if (!Number.isNaN(stockIdToRemove) && stockIdToRemove > 0) {
                    const index = self.getIndexStockById(stockIdToRemove)
                    self.stockList[index] = result.Data

                    self.replaceStockInList(result.Data, stockIdToRemove)

                } else {
                    if (self.stockList.length == 0)
                        self.clearStockList()

                    self.stockList.push(result.Data);
                    self.addStockToList(result.Data);
                }

                cancelDialog("#stockDialog");
            } else {
                showErrorMessage(result.ErrorMessage)
            }
        }

        $.post("/Admin/SaveStock", stock, successCallBack(successFunc, loader))
    },
    processingLoadStockData: function (data) {
        data.StockFromDate = jsonToDate(data.StockFromDate)
        data.StockToDate = jsonToDate(data.StockToDate)
        data.ConditionCountProducts = JSON.parse(data.ConditionCountProductsJSON);
    },
    clearStockList: function () {
        $("#stock-list").empty();
    },
    cleanStockDialog: function () {
        this.productsCountConditional = {}

        $('#stockDialog select').not('.promotion-custom-select').each(function (i) {
            const sumo = $(this)[0].sumo

            if (sumo)
                sumo.reload()
        })

        $('#stockDialog').attr('stock-id', -1)
        $('#stockDialog .promotion-stock-index-block.hide-block').hide()
        $('#stockDialog .promotion-stock-dialog-slide').hide()
        $('#stockDialog #stock-slide-1').show()
        $('#stockDialog img').removeAttr('src').addClass('hide')
        $('#stockDialog .dialog-image-upload').removeClass('hide')
        $('#stockDialog textarea').val('')
        $('#stockDialog input[type=text]').val('')
        $('#stockDialog input[type=file]').val('')
        $('#stock-discount-val').val('')
        $('#stock-products-count').val(1)
        $('#bonus-product-items')[0].sumo.reload()
        $('#stock-excluded-products-items')[0].sumo.reload()
        if ($('#discount-type')[0].sumo) {
            $('#discount-type')[0].sumo.selectItem('1')
        }
        $('#stock-condition-sum-count').val('')
        $('#condition-product-items')[0].sumo.reload()
        $('#stock-condition-products-count-container .stock-setting-condition-count-products').html('')
        $(".promotion-stock-next").attr('disabled', true)

        let datePicker = $("#stock-type-calendar-period").data("datepicker");
        const fromDate = new Date()
        const toDate = new Date()
        toDate.setDate(toDate.getDate() + 7)

        datePicker.selectDate([fromDate, toDate]);
    },
    editStock: function (id) {
        this.cleanStockDialog()
        this.showStock(id)
    },
    getStockById: function (searchId) {
        for (let id in this.stockList) {
            if (this.stockList[id].Id == searchId) {
                return this.stockList[id];
            }
        }
    },
    removeStock: function (id) {
        const self = this
        $(`.stock-item[stock-id=${id}]`).fadeOut(1000, function () {
            $(this).remove();

            const stockIndex = self.getIndexStockById(id);
            self.stockList.splice(stockIndex, 1);

            if (self.stockList.length == 0)
                self.setEmptyStockInfo()
        });

        $.post("/Admin/RemoveStock", { id: id }, null);
    },
    getIndexStockById: function (searchId) {
        for (let id in this.stockList) {
            if (this.stockList[id].Id == searchId) {
                return id;
            }
        }
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
                case RewardType.Discount:
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
                case StockConditionTriggerType.HappyBirthday:
                    const periodBefore = parseInt($('#stock-condition-birthday-period-before').val())
                    const periodAfter = parseInt($('#stock-condition-birthday-period-after').val())

                    if (!Number.isNaN(periodBefore) && periodBefore >= 0 &&
                        !Number.isNaN(periodAfter) && periodAfter >= 0)
                        enabledNextAction()
                    else
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
                case RewardType.Discount:
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
                    $('#stock-type-products-excluded-container').show(animationOption, '', 150)
                    break
                case StockConditionTriggerType.ProductsOrder:
                    $('#stock-condition-products-container').show(
                        animationOption,
                        '',
                        150,
                        this.onStockConditionProductsChange())
                    break
                case StockConditionTriggerType.HappyBirthday:
                    $('#stock-condition-birthday-container').show(animationOption, '', 150)
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
    onStockConditionBirthdayPeriod: function () {
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
            const countItems = this.getProductCountConditionalItems()

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
    getProductCountConditionalItems: function () {
        const countItems = []

        for (let productId in this.productsCountConditional) {
            const data = this.productsCountConditional[productId]
            const product = ProductsForPromotion[data.categoryId].filter(p => p.Id == productId)[0]

            const span = `<span product-id="${productId}" title="${product.Name}">${product.Name}</span>`
            const iNumber = `<input 
                                    onfocusout="StockManger.onStockConditionProductsFocusOut(${productId}, this)"
                                    onchange="StockManger.onStockConditionProductCountChange(${productId}, this)"
                                    type="number"
                                    min="1"
                                    value="${data.count}">`
            const row = `<div class="stock-product-count-item">${span}${iNumber}</div>`

            countItems.push(row)
        }

        return countItems
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
    },
}
