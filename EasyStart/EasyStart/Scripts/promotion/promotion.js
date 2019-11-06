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
        $stockDialog.find('select').SumoSelect()
    },
    saveStockFromDialog: function () {
    },
    cleanStockDialog: function () {
        $('#stockDialog select').each(function (i) {
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
        ConditionType: 3
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
                    $("#stock-slide-2 .promotion-stock-next").attr('disabled', true)
                    break
            }
        }

        switch (slide) {
            case this.slide.StockType:
                stockTypeToggle()
                break
            case this.slide.RewardType:
                rewardTypeToggle()
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
    }
}



