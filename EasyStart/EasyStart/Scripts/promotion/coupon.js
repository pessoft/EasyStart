var CouponManager = {
    coupons: [],
    loadCoupons: function () {
        const self = this

        this.cleanCouponDialog();

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
        data.FromDate = jsonToDate(data.FromDate)
        data.ToDate = jsonToDate(data.ToDate)
    },
    addAllItemCoupons: function (data) {
        for (let coupon of data) {
            this.addCouponToList(coupon);
        }
    },
    addCouponToList: function (data) {
        let $template = this.getRowCouponTemplate(data)

        $("#coupon-list").append($template);
    },
    getRowCouponTemplate: function (data) {

    },
    addNewCoupon: function () {
        this.cleanCouponDialog()
        this.showCouponDialog()
    },
    saveCoupone: function () {

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
    },
    setCouponDataInDialog: function (id) {
        const data = this.getCouponById(id)

        if (!data)
            return

        // set data in dialog
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
    btnNextToggle: function (slide) {
        const self = this

        const generalSettingToggle = () => {
            const name = $('#promotion-coupon-name').val().trim()
            const promocode = $('#promotion-coupon-code').val().trim()

            if (name && promocode)
                $("#coupon-slide-1 .promotion-coupon-next").removeAttr('disabled')
            else
                $("#coupon-slide-1 .promotion-coupon-next").attr('disabled', true)
        }

        const rewardSettingToggle = () => {

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
    onRewardChangeType: function () {
        const animationOption = { effect: "scale", direction: "horizontal" }
        const rewardType = parseInt($('#coupon-type-reward option:selected').val())
        const callback = () => {
            switch (rewardType) {
                case RewardType.Discout:
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
        this.btnNextToggle(this.slide.RewardType)
    },
}