$(document).ready(function () {
    $('.promotion-menu li').bind('click', function () { changePromotionActiveMenu(this) })
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
        const $e = $('#stock-type-period')
        const animationOption = { effect: "scale", direction: "horizontal"}

        if ($e.find('option:selected').val() == SotckTypePeriod.OneOff) {
            $('#stock-one-type-subtype-container').show(animationOption, '', 250)

        } else {
            $('#stock-one-type-subtype-container').hide(animationOption, '', 250)
        }

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
                $("#stock-slide-1 .promotion-stock-next").removeAttr('disabled')
                break
            case SotckTypePeriod.ToDate:
                $("#stock-slide-1 .promotion-stock-next").attr('disabled', true)
                break
        }
    }
}



