var CouponManager = {
    coupons: [],
    loadCoupons: function () {
        const self = this

        this.clearCouponList();

        if (this.coupons &&
            this.coupons.length > 0) {
            this.addAllItemCoupons(this.coupons);
        } else {
            let loader = new Loader($("#coupon-list"));
            let successFunc = function (result, loader) {
                loader.stop();
                if (result.Success) {
                    if (!result.Data || result.Data.length == 0) {
                        self.setEmptyCouponInfo();
                    } else {
                        result.Data.forEach(p => self.processingLoadCouponData(p))
                        self.coupons = result.Data;
                        self.addAllItemCoupons(self.coupons);
                    }
                } else {
                    showErrorMessage(result.ErrorMessage);
                    self.setEmptyCouponInfo();
                }
            }

            loader.start();

            $.post("/Admin/LoadCoupons", null, successCallBack(successFunc, loader));
        }
    },
    processingLoadCouponData: function (data) {
        data.DateFrom = jsonToDate(data.DateFrom)
        data.DateTo = jsonToDate(data.DateTo)
    },
    addAllItemCoupons: function (data) {
        const coupons = []
        for (let coupon of data) {
            const $coupon = this.getRowCouponTemplate(coupon)
            coupons.push($coupon)
        }

        this.addCouponToContainer(coupons)
    },
    addCouponToContainer: function (items) {
        $("#coupon-list").append(items);
    },
    addCouponToList: function (data) {
        let $template = this.getRowCouponTemplate(data)
        this.addCouponToContainer($template)
    },
    getRowCouponTemplate: function (data) {
        const $template = $($("#coupon-item-template").html())

        $template.attr('coupon-id', data.Id)

        $template.find('.coupon-item-name span').html(data.Name)
        $template.find('.coupon-item-period').html(`${data.DateFrom.toLocaleDateString()} - ${data.DateTo.toLocaleDateString()}`)
        $template.find('.coupon-item-promocode').html(data.Promocode)
        $template.find('.coupon-item-count').html(`${data.CountUsed}/${data.Count}`)

        $template.find('.edit-btn').bind('click', () => CouponManager.editCoupon(data.Id))
        $template.find('.remove-btn').bind('click', () => CouponManager.removeCoupon(data.Id))

        return $template
    },
    addNewCoupon: function () {
        this.cleanCouponDialog()
        this.showCouponDialog()
    },
    saveCoupon: function () {
        let loader = new Loader($("#couponDialog .custom-dialog-body"));
        const self = this

        loader.start();

        const getAllowedBonusProductsJSON = () => {
            const prodictIds = []
            $('#coupon-bonus-products-items option:selected').each(function () {
                prodictIds.push(parseInt($(this).val()))
            })

            return JSON.stringify(prodictIds)
        }

        const getIntValue = str => {
            let value = parseInt(str)

            if (Number.isNaN(value))
                value = 0

            return value
        }

        const couponIdToRemove = parseInt($('#couponDialog').attr('coupon-id'))
        let coupon = {
            id: couponIdToRemove,
            name: $('#promotion-coupon-name').val(),
            dateFrom: $("#coupon-calendar-period").data("datepicker").selectedDates[0].toJSON(),
            dateTo: $("#coupon-calendar-period").data("datepicker").selectedDates[1].toJSON(),
            promocode: $('#promotion-coupon-promocode').val(),
            count: parseInt($('#promotion-coupon-count').val()),
            rewardType: parseInt($('#coupon-type-reward option:selected').val()),
            discountValue: getIntValue($('#coupon-discount-val').val()),
            discountType: parseInt($('#coupon-discount-type option:selected').val()),
            countBonusProducts: parseInt($('#coupon-products-count').val()),
            allowedBonusProductsJSON: getAllowedBonusProductsJSON(),
            isOneCouponOneClient: $('#toggle-one-coupon-one-client').is(':checked')
        }

        const successFunc = function (result, loader) {
            loader.stop();
            if (result.Success) {
                $("#coupon-list .empty-list").remove();
                self.processingLoadCouponData(result.Data)

                if (!Number.isNaN(couponIdToRemove) && couponIdToRemove > 0) {
                    const index = self.getIndexCouponById(couponIdToRemove)
                    self.coupons[index] = result.Data

                    self.replaceModifiedCoupon(result.Data, couponIdToRemove)

                } else {
                    if (self.coupons.length == 0)
                        self.clearCouponList()

                    self.coupons.push(result.Data);
                    self.addCouponToList(result.Data);
                }

                cancelDialog("#couponDialog");
            } else {
                showErrorMessage(result.ErrorMessage)
            }
        }

        $.post("/Admin/SaveCoupon", coupon, successCallBack(successFunc, loader))
    },
    replaceModifiedCoupon: function (data, replaceCouponId) {
        let $template = this.getRowCouponTemplate(data)

        $(`#coupon-list [coupon-id="${replaceCouponId}"]`).replaceWith($template);
    },
    editCoupon: function (id) {
        this.cleanCouponDialog()
        this.setCouponDataInDialog(id)
        this.showCouponDialog()
    },
    removeCoupon: function (id) {
        const self = this
        $(`.coupon-item[coupon-id=${id}]`).fadeOut(1000, function () {
            $(this).remove();

            const couponIndex = self.getIndexCouponById(id);
            self.coupons.splice(couponIndex, 1);

            if (self.coupons.length == 0)
                self.setEmptyCouponInfo()
        });

        $.post("/Admin/RemoveCoupon", { id: id }, null);
    },
    showCouponDialog: function () {
        let $couponDialog = $('#couponDialog')

        Dialog.showModal($couponDialog)
        $couponDialog.find('select').not('.promotion-custom-select').SumoSelect()
    },
    clearCouponList: function () {
        $("#coupon-list").empty();
    },
    cleanCouponDialog: function () {
        $('#couponDialog .promotion-coupon-dialog-slide').hide()
        $('#couponDialog #coupon-slide-1').show()
        $('#couponDialog').attr('coupon-id', -1)

        $('#couponDialog select').not('.promotion-custom-select').each(function (i) {
            const sumo = $(this)[0].sumo

            if (sumo)
                sumo.unload()
        })

        $('#couponDialog .promotion-coupon-index-block.hide-block').hide()

        $('#couponDialog select option').removeAttr('disabled')
        $('#couponDialog select option').removeAttr('selected')
        $('#couponDialog select option[value=0]').attr('selected', true)
        $('#couponDialog select option[value=0]').attr('disabled', true)

        $(".promotion-coupon-next").attr('disabled', true)

        $('#promotion-coupon-name').val('')
        $('#promotion-coupon-promocode').val('')
        $('#promotion-coupon-count').val(1)

        $('#coupon-discount-val').val('')
        $('#coupon-products-count').val(1)
        $('#coupon-bonus-products-items')[0].sumo.unSelectAll()
        $('#coupon-discount-type').removeAttr('selected').find('option[value=1]').attr('selected', true)

        $('#toggle-one-coupon-one-client').prop('checked', false)

        let datePicker = $("#coupon-calendar-period").data("datepicker");
        const fromDate = new Date()
        const toDate = new Date()
        toDate.setDate(toDate.getDate() + 7)

        datePicker.selectDate([fromDate, toDate]);
    },
    setCouponDataInDialog: function (id) {
        const data = this.getCouponById(id)

        if (!data)
            return

        $('#couponDialog').attr('coupon-id', id)
        $(".promotion-coupon-next").removeAttr('disabled')

        this.setGeneralSettingData(data)
        this.setRewardSettingData(data)
    },
    setGeneralSettingData: function (data) {
        $('#promotion-coupon-name').val(data.Name)
        $('#promotion-coupon-promocode').val(data.Promocode)
        $('#promotion-coupon-count').val(data.Count)
        $('#toggle-one-coupon-one-client').prop('checked', data.IsOneCouponOneClient)

        const datePicker = $("#coupon-calendar-period").data("datepicker");
        datePicker.selectDate([data.DateFrom, data.DateTo]);
    },
    setRewardSettingData: function (data) {
        $(`#coupon-type-reward option[value="${data.RewardType}"]`).attr('selected', true)

        switch (data.RewardType) {
            case RewardType.Discount:
                $('#coupon-type-discount-container').show()

                $('#coupon-discount-val').val(data.DiscountValue)
                $(`#coupon-discount-type option[value="${data.DiscountType}"]`).attr('selected', true)
                break;
            case RewardType.Products:
                $('#coupon-type-products-container').show()

                $('#coupon-products-count').val(data.CountBonusProducts)
                data.AllowedBonusProducts.forEach(p => {
                    $(`#coupon-bonus-products-items`)[0].sumo.selectItem(p.toString())
                })
                break;
        }
    },
    getCouponById: function (id) {
        const coupons = this.coupons.filter(p => p.Id == id)

        return coupons && coupons.length ? coupons[0] : null
    },
    getIndexCouponById: function (searchId) {
        for (let id in this.coupons) {
            if (this.coupons[id].Id == searchId) {
                return id;
            }
        }
    },
    setEmptyCouponInfo: function () {
        let template = `
        <div class="empty-list">
            <i class="fal fa-smile-plus"></i>
            <span>Добавте купоны</span>
        </div>`;

        $("#coupon-list").append(template);
    },
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
        let $inputDate = $("#coupon-calendar-period");
        $inputDate.datepicker(options);
        datePicker = $inputDate.data("datepicker");

        $inputDate.next("i").bind("click", function () {
            datePicker.show();
        })

        const fromDate = new Date()
        const toDate = new Date()
        toDate.setDate(toDate.getDate() + 7)

        datePicker.selectDate([fromDate, toDate]);
    },
    slide: {
        CouponGeneralSetting: 1,
        RewardSetting: 2,
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
    isValidPromocode: function (promocode) {
        const couponId = parseInt($('#couponDialog').attr('coupon-id'))
        const valid = promocode &&
            this.coupons.filter(p => p.Promocode == promocode &&
                (couponId == -1 || p.Id != couponId)).length == 0

        return valid
    },
    btnNextToggle: function (slide) {
        const self = this

        const generalSettingToggle = () => {
            const name = $('#promotion-coupon-name').val().trim()
            const promocode = $('#promotion-coupon-promocode').val().trim()
            const count = parseInt($('#promotion-coupon-count').val())
            const maxCount = 10000

            if (name && self.isValidPromocode(promocode) && count > 0 && count <= maxCount)
                $("#coupon-slide-1 .promotion-coupon-next").removeAttr('disabled')
            else
                $("#coupon-slide-1 .promotion-coupon-next").attr('disabled', true)
        }

        const rewardSettingToggle = () => {
            const rewardType = parseInt($('#coupon-type-reward option:selected').val())

            switch (rewardType) {
                case RewardType.Discount:
                    const val = parseInt($('#coupon-discount-val').val())

                    if (Number.isNaN(val) || val == 0)
                        $("#coupon-slide-2 .promotion-coupon-next").attr('disabled', true)
                    else
                        $("#coupon-slide-2 .promotion-coupon-next").removeAttr('disabled')

                    break
                case RewardType.Products:
                    const allowedCountProducts = parseInt($('#coupon-products-count').val())
                    const countSelectedProducts = $('#coupon-bonus-products-items option:selected').length

                    if (Number.isNaN(allowedCountProducts)
                        || allowedCountProducts < 1
                        || countSelectedProducts == 0)
                        $("#coupon-slide-2 .promotion-coupon-next").attr('disabled', true)
                    else
                        $("#coupon-slide-2 .promotion-coupon-next").removeAttr('disabled')
                    break
            }
        }

        switch (slide) {
            case this.slide.CouponGeneralSetting:
                generalSettingToggle()
                break
            case this.slide.RewardSetting:
                rewardSettingToggle()
                break
        }
    },
    onCouponNameChange: function () {
        this.btnNextToggle(this.slide.CouponGeneralSetting)
    },
    onCouponCodeChange: function () {
        this.btnNextToggle(this.slide.CouponGeneralSetting)
    },
    onCouponCountChange: function () {
        this.btnNextToggle(this.slide.CouponGeneralSetting)
    },
    onRewardChangeType: function () {
        const animationOption = { effect: "scale", direction: "horizontal" }
        const rewardType = parseInt($('#coupon-type-reward option:selected').val())
        const callback = () => {
            switch (rewardType) {
                case RewardType.Discount:
                    $('#coupon-type-discount-container').show(animationOption, '', 150)
                    break
                case RewardType.Products:
                    $('#coupon-type-products-container').show(animationOption, '', 150)
                    break
            }
        }

        const $hideBlock = $('#coupon-slide-2 .promotion-coupon-index-block.hide-block:visible')
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

        this.btnNextToggle(this.slide.RewardSetting)
    },
    onBonusProductsChange: function () {
        this.btnNextToggle(this.slide.RewardSetting)
    },
    onDicountChange: function () {
        const discountType = parseInt($('#coupon-discount-type option:selected').val())
        let val = parseInt($('#coupon-discount-val').val())

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

        $('#coupon-discount-val').val(val)

        this.btnNextToggle(this.slide.RewardSetting)
    },
    onAllowedBonusCountChange: function () {
        this.btnNextToggle(this.slide.RewardSetting)
    },
    generatePromocode: function (length = 6) {
        const promocode = Math.random().toString(36).slice(-length);

        $('#promotion-coupon-promocode').val(promocode)
        this.onCouponCodeChange()
    }
}