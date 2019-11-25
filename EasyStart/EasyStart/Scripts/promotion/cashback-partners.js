var CashbackPartners = {
    loadCashbackPartners: function () {

    },
    toggleCashbackBonus: function () {
        const idContainer = 'promotion-cashback-bonus'

        if ($('#toggle-cashback-bonus').is(':checked')) {
            $(`#${idContainer} .group input`).removeAttr('disabled')
        } else {
            $('#toggle-partners').prop('checked', false)
            $(`#${idContainer} .group input`).attr('disabled', true)
            $('#cashback-bonus-return').val(10)
            $('#payment-cashback-bonus').val(40)
            $('#save-cashback-bonus-setting').removeAttr('disabled')
        }
    },
    onChangeCashbackBonusPersentValue: function () {
        const returnedValue = parseInt($('#cashback-bonus-return').val())
        const paymentValue = parseInt($('#payment-cashback-bonus').val())
        const $btnSave = $('#save-cashback-bonus-setting')

        if (Number.isNaN(returnedValue) || returnedValue < 0 || returnedValue > 100
            || Number.isNaN(paymentValue) || paymentValue < 0 || paymentValue > 100)
            $btnSave.attr('disabled', true)
        else
            $btnSave.removeAttr('disabled')
    },
    onChangePaymentCashbackBonusValue: function () {
        const returnedReferalValue = parseInt($('#partner-cashback-bonus').val())
        const bonusValue = parseInt($('#partner-cashback-bonus-return').val())
        const typeBonusValue = parseInt($('#partner-cashback-type option:selected').val())
        const $btnSave = $('#save-partner-setting')

        if (Number.isNaN(returnedReferalValue) || returnedReferalValue < 0 || returnedReferalValue > 100
            || Number.isNaN(bonusValue) || bonusValue < 0 || (typeBonusValue == DiscountType.Percent &&  bonusValue > 100))
            $btnSave.attr('disabled', true)
        else
            $btnSave.removeAttr('disabled')
    },
    togglePartners: function () {
        const idContainer = 'promotion-partners'
        const message = 'Для партнерской программы активируйте функцию кешбека'

        if ($('#toggle-partners').is(':checked') && !$('#toggle-cashback-bonus').is(':checked')) {
            $('#toggle-partners').prop('checked', false)
            showInfoConfirm(message)
        } else if ($('#toggle-partners').is(':checked')) {
            $(`#${idContainer} .group input,#${idContainer} select`).removeAttr('disabled')
        } else {
            $(`#${idContainer} .group input,#${idContainer} select`).attr('disabled', true)
        }
    }
}