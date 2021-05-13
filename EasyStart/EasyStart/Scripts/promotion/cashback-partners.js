var CashbackPartners = {
    cashbackSetting: null,
    partnersSetting: null,
    loadCashbackPartnerSettings: function () {
        let loader = new Loader($("#promotion-cashback-partners"));
        const self = this

        loader.start();

        const successFunc = function (result, loader) {
            loader.stop();

            if (result.Success) {
                self.cashbackSetting = result.Data.CashbackSetting
                self.partnersSetting = result.Data.PartnersSetting

                self.setCasbackSetting()
                self.setPartnerSetting()
            } else {
                showErrorMessage(result.ErrorMessage);
            }
        }

        $.post("/Admin/LoadCashbackPartnerSettings", null, successCallBack(successFunc, loader));
    },
    saveCashbackSetting: function () {
        let loader = new Loader($("#promotion-cashback-bonus"));
        const self = this

        loader.start();

        const isUseCashback = $('#toggle-cashback-bonus').is(':checked')
        const cashbackSetting = {
            id: this.cashbackSetting ? this.cashbackSetting.Id : -1,
            isUseCashback: isUseCashback,
            returnedValue: isUseCashback ? $('#cashback-bonus-return').val() : 0,
            paymentValue: isUseCashback ? $('#payment-cashback-bonus').val() : 0,
            alwaysApplyCashback: isUseCashback ? $('#toggle-cashback-always').is(':checked') : false,
        }

        const successFunc = function (result, loader) {
            loader.stop();
            if (result.Success) {
                self.cashbackSetting = result.Data

                if (!self.cashbackSetting.IsUseCashback) {
                    $('#toggle-partners').prop('checked', false)
                    self.togglePartners()

                    if (self.partnersSetting && self.partnersSetting.IsUsePartners) {
                        self.savePartnerSetting()
                    }

                }

            } else {
                showErrorMessage(result.ErrorMessage)
            }
        }

        $.post("/Admin/SavePromotionCashbackSetting", cashbackSetting, successCallBack(successFunc, loader))
    },
    savePartnerSetting: function () {
        let loader = new Loader($("#promotion-partners"));
        const self = this

        loader.start();

        const isUsePartners = $('#toggle-partners').is(':checked')
        const partnerSetting = {
            id: this.partnersSetting ? this.partnersSetting.Id : -1,
            isUsePartners: isUsePartners,
            cashBackReferralValue: isUsePartners ? $('#partner-cashback-bonus').val() : 0,
            isCashBackReferralOnce: $('#partner-cashback-once').is(':checked'),
            typeBonusValue: isUsePartners ? $('#partner-cashback-type option:selected').val() : 1,
            bonusValue: isUsePartners ? $('#partner-cashback-bonus-return').val() : 0,
        }

        const successFunc = function (result, loader) {
            loader.stop();
            if (result.Success) {
                self.partnersSetting = result.Data
            } else {
                showErrorMessage(result.ErrorMessage)
            }
        }

        $.post("/Admin/SavePromotionPartnerSetting", partnerSetting, successCallBack(successFunc, loader))
    },
    setCasbackSetting: function () {
        if (this.cashbackSetting && Object.keys(this.cashbackSetting).length > 0) {
            $('#toggle-cashback-bonus').prop('checked', this.cashbackSetting.IsUseCashback)

            if (this.cashbackSetting.IsUseCashback) {
                $('#promotion-cashback-bonus .group input').removeAttr('disabled')
                $('#cashback-bonus-return').val(this.cashbackSetting.ReturnedValue)
                $('#payment-cashback-bonus').val(this.cashbackSetting.PaymentValue)
                $('#toggle-cashback-always').prop('checked', this.cashbackSetting.AlwaysApplyCashback)
            } else {
                $('#promotion-cashback-bonus .group input').attr('disabled', true)
                $('#cashback-bonus-return').val(10)
                $('#payment-cashback-bonus').val(40)
                $('#toggle-cashback-always').prop('checked', false)
            }
        }
    },
    setPartnerSetting: function () {
        const idContainer = 'promotion-partners'
        const $select = $('#partner-cashback-type')

        if (this.partnersSetting && Object.keys(this.partnersSetting).length > 0) {
            $('#toggle-partners').prop('checked', this.partnersSetting.IsUsePartners)

            if (this.partnersSetting.IsUsePartners) {
                $(`#${idContainer} .row input,#${idContainer} select`).removeAttr('disabled')
                $select.find(`option[value="${this.partnersSetting.TypeBonusValue}"]`).attr('selected', true)

                $('#partner-cashback-once').prop('checked', this.partnersSetting.IsCashBackReferralOnce)
                $('#partner-cashback-bonus').val(this.partnersSetting.CashBackReferralValue)
                $('#partner-cashback-bonus-return').val(this.partnersSetting.BonusValue)
            } else {
                $select.attr('disabled', true)
                $select.find('option[value="1"]').attr('selected', true)

                $(`#${idContainer} .row input,#${idContainer} select`).attr('disabled', true)
                $('#partner-cashback-once').prop('checked', true)
                $('#partner-cashback-bonus').val(20)
                $('#partner-cashback-bonus-return').val(20)
            }
        }
    },
    toggleCashbackBonus: function () {
        const idContainer = 'promotion-cashback-bonus'

        if ($('#toggle-cashback-bonus').is(':checked')) {
            $(`#${idContainer} .group input`).removeAttr('disabled')
        } else {
            $('#toggle-partners').prop('checked', false)
            this.togglePartners()

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
            || Number.isNaN(bonusValue) || bonusValue < 0 || (typeBonusValue == DiscountType.Percent && bonusValue > 100))
            $btnSave.attr('disabled', true)
        else
            $btnSave.removeAttr('disabled')
    },
    togglePartners: function () {
        const idContainer = 'promotion-partners'
        const message = 'Для партнерской программы активируйте функцию кешбека'

        if ($('#toggle-partners').is(':checked')
            && ((this.cashbackSetting && !this.cashbackSetting.IsUseCashback) || !this.cashbackSetting)) {
            $('#toggle-partners').prop('checked', false)
            showInfoConfirm(message)
        } else if ($('#toggle-partners').is(':checked')) {
            $(`#${idContainer} .row input,#${idContainer} select`).removeAttr('disabled')
        } else {
            const $select = $('#partner-cashback-type')
            $select.attr('disabled', true)
            $select.find('option[value="1"]').attr('selected', true)

            $(`#${idContainer} .row input,#${idContainer} select`).attr('disabled', true)
            $('#partner-cashback-once').prop('checked', true)
            $('#partner-cashback-bonus').val(20)
            $('#partner-cashback-bonus-return').val(20)
        }
    }
}