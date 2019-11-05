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

        this.btnNextToggle()
    },
    btnNextToggle: function () {
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
}



