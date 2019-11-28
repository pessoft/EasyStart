var PromotionSetting = {
    loadSettings: function () {
        let loader = new Loader($("#promotion-settings"));
        const self = this

        loader.start();

        const successFunc = function (result, loader) {
            loader.stop();

            if (result.Success) {
                self.setSettings(result.Data)
            } else {
                showErrorMessage(result.ErrorMessage);
            }
        }

        $.post("/Admin/LoadPromotionSettings", null, successCallBack(successFunc, loader));
    },
    saveSettings: function () {
        let loader = new Loader($("#promotion-settings"));
        const self = this

        loader.start();

        const successFunc = function (result, loader) {
            if (result.Success) {
                self.setSettings(result.Data)
            } else {
                showErrorMessage(result.ErrorMessage);
            }

            loader.stop();
        }

        const settings = this.getSettings()

        if (!settings || settings.length == 0) {
            loader.stop();
            showErrorMessage("Попытка сохранения пустой настройки");
            return
        }

        $.post("/Admin/SavePromotionSettings", { settings }, successCallBack(successFunc, loader));
    },
    bindDragula: function () {
        dragula([document.getElementById("promotion-items-priority")], {
            revertOnSpill: true
        })
    },
    setSettings: function (settings) {
        if (!settings || settings.length == 0)
            return

        

        const uiItems = []
        for (let setting of settings) {
            const $item = $(`#promotion-items-priority .promotion-item-setting[promotion-section="${setting.PromotionSection}"]`)
            const sumo = $item.find('select')[0].sumo

            $item.attr('promotion-setting-id', setting.Id)
            
            for (var key in PromotionSection) {
                const section = PromotionSection[key]
                if (BitOperation.isHas(setting.Intersections, section)) {
                    sumo.selectItem(section.toString())
                } else {
                    sumo.unSelectItem(section.toString())
                }
            }

            uiItems.push($item)
        }

        $('#promotion-items-priority').html(uiItems)
        this.unbinSumoSelectForPromotionSettings()
        this.binSumoSelectForPromotionSettings()
    },
    getSettings: function () {
        const settings = []

        $('#promotion-items-priority .promotion-item-setting').each(function (i) {
            const $e = $(this)
            const id = $e.attr('promotion-setting-id')
            const promotionSection = parseInt($e.attr('promotion-section'))
            let intersections = PromotionSection.Unknown

            $e.find('select option:selected').each(function () {
                const additionalIntersection = parseInt($(this).val())

                intersections = BitOperation.Add(intersections, additionalIntersection)
            })

            settings.push({
                Id: id,
                Priorety: i,
                PromotionSection: promotionSection,
                Intersections: intersections
            })
        })

        return settings
    },
    onChangeIntesections: function (e) {
        const $e = $(e).parents('.promotion-item-setting')
        const currentPromotionSection = $e.attr('promotion-section')

        $e.find('select option').each(function () {
            const $self = $(this)
            const isSelected = $self.is(':selected')
            const promotionSection = $self.val()

            const $targetSection = $(`#promotion-items-priority .promotion-item-setting[promotion-section=${promotionSection}]`)

            if (isSelected)
                $targetSection.find('select')[0].sumo.selectItem(currentPromotionSection)
            else
                $targetSection.find('select')[0].sumo.unSelectItem(currentPromotionSection)
        })
    },
    unbinSumoSelectForPromotionSettings: function () {
        $('#promotion-settings select').each(function () {
            const sumo = $(this)[0].sumo

            if (sumo)
                sumo.unload()
        })
    },
    binSumoSelectForPromotionSettings: function () {
        const sumoOptions = {
            placeholder: 'Выберите пересечения',
            captionFormat: 'Пересекающихся разделов: {0}',
            captionFormatAllSelected: 'Учитывать все разделы: {0}'
        }
        $('#promotion-settings select').SumoSelect(sumoOptions)
    }
}