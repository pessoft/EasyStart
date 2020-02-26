var SumoSelects = {
    PushActinos: null,
    PushAdditionalAction: null
}

function bindPushNotificationEvent() {
    $('#push-title').bind('input', () => PushDataHandler.toggleBtnSave())
    $('#push-body').bind('input', () => PushDataHandler.toggleBtnSave())
    $('#push-additional-action').bind('change', () => PushDataHandler.toggleBtnSave())
    $('#send-new-push-message').bind('click', () => PushDataHandler.sendNewPushMessage())
}

function bindPushImagePreview() {
    $("#push-image-download").change(function () {
        addPreviewPushImage(this)
    });
}

function bindSumoselectAction() {
    const sumoOptions = {
        search: true,
        searchText: 'Поиск...',
        noMatch: 'Нет совпадений для "{0}"',
    }

    SumoSelects.PushActinos = $($('#push-actions').SumoSelect()).parents('.SumoSelect')
    SumoSelects.PushAdditionalAction = $($('#push-additional-action').SumoSelect(sumoOptions)).parents('.SumoSelect')

    SumoSelects.PushAdditionalAction.hide()
}

function setDefaultPushNotification() {
    DataCollectorNewMessage.setDefaultValues()
    PreviewDevice.setDefaultValues()
    NotificationAction.setDefaultAction()
    PushNotitifactionImage.setPreviewImage()
    PushDataHandler.toggleBtnSave()
}

function setPushMessage(data) {
    DataCollectorNewMessage.setDefaultValues(data.Title, data.Body)
    PreviewDevice.setDefaultValues(data.Title, data.Body, data.ImageUrl)
    NotificationAction.setDefaultAction(getDataAction(data))
    PushNotitifactionImage.setPreviewImage(data.ImageUrl)
    PushDataHandler.toggleBtnSave()
}

function getDataAction(data) {
    const dataFromJson = JSON.parse(data.DataJSON)
    const payload = JSON.parse(dataFromJson.payload)
    const action = payload.action

    return action
}

const NotificationActionType = {
    NoAction: 0,
    OpenCategory: 1,
    OpenProductInfo: 2,
    OpenStock: 3,
    OpenPartners: 4,
    OpenCashback: 5
}

var PreviewDevice = {
    setDefaultValues: function (title, message, imageUrl) {
        this.setMessageTitle(title)
        this.setMessageBody(message)
        this.setMessageImage(imageUrl)
    },
    setMessageTitle: function (value) {
        const title = value ? value : 'Заголовок сообщения'
        const query = '.device-preview-container .preview-msg-title'

        $(query).html(title)
    },
    setMessageBody: function (value) {
        const body = value ? value : 'Текст сообщения'
        const query = '.device-preview-container .preview-msg-body-text'

        $(query).html(body)
    },
    setMessageImage: function (value) {
        const src = value ? value : '/images/default-image.jpg'
        const query = '.device-preview-container .preview-msg-image-device img'
        const cssClassImageLoaded = 'preview-msg-image-loaded'
        const $img = $(query)

        if (value)
            $img.addClass(cssClassImageLoaded)
        else
            $img.removeClass(cssClassImageLoaded)

        $img.attr('src', src)
    },
    onInputMessageTitle: function (e) {
        const title = $(e).val().trim()

        this.setMessageTitle(title)
    },
    onInputMessageBody: function (e) {
        const body = $(e).val().trim()

        this.setMessageBody(body)
    }
}

var NotificationAction = {
    animationSpeed: 200,
    setDefaultAction: function (defaultAction) {
        const query = '#push-actions'
        const actionType = defaultAction ? defaultAction.type : NotificationActionType.NoAction
        const defaultTargetId = defaultAction ? defaultAction.targetId : -1
        $(query)[0].sumo.selectItem(actionType)
        this.onChangeAction(query, defaultTargetId)
    },
    onChangeAction: function (e, defaultAdditionalTargetId) {
        const actionType = parseInt($(e).children('option:selected').val())

        this.toggleAdditionalSelectControl(actionType, defaultAdditionalTargetId)
    },
    toggleAdditionalSelectControl: function (actionType, defaultAdditionalTargetId) {
        switch (actionType) {
            case NotificationActionType.NoAction:
            case NotificationActionType.OpenCashback:
            case NotificationActionType.OpenPartners:
                SumoSelects.PushAdditionalAction.hide(this.animationSpeed)
                break
            case NotificationActionType.OpenCategory:
                this.setOpenCategoryAction(defaultAdditionalTargetId)
                break
            case NotificationActionType.OpenProductInfo:
                this.setOpenProductAction(defaultAdditionalTargetId)
                break
            case NotificationActionType.OpenStock:
                this.setOpenStockAction(defaultAdditionalTargetId)
                break
        }
    },
    setOpenCategoryAction: function (defaultAdditionalTargetId) {
        let options = []
        const fiersItemIsSelected = defaultAdditionalTargetId && defaultAdditionalTargetId > 0 ? '' : 'selected'

        options.push(`<option value='-1' disabled ${fiersItemIsSelected}>Выберите категорию</option>`)
        for (let id in CategoryDictionary) {
            const categoryName = CategoryDictionary[id]
            const selected = defaultAdditionalTargetId == id ? 'selected' : ''

            options.push(`<option value='${id}' ${selected}>${categoryName}</option>`)
        }

        this.setDataAdditioanlAction(options)
    },
    setOpenProductAction: function (defaultAdditionalTargetId) {
        let optGroups = []
        const fiersItemIsSelected = defaultAdditionalTargetId && defaultAdditionalTargetId > 0 ? '' : 'selected'

        optGroups.push(`<option value='-1' disabled ${fiersItemIsSelected}>Выберите блюдо</option>`)
        for (let id in CategoryDictionary) {
            const categoryName = CategoryDictionary[id]
            const $optGroup = $(`<optgroup label='${categoryName}'></optgroup>`)
            const products = ProductsForPromotion[id]
            let options = []

            

            for (product of products) {
                const selected = defaultAdditionalTargetId == product.Id ? 'selected' : ''

                options.push(`<option value='${product.Id}' ${selected}>${product.Name}</option>`)
            }

            $optGroup.html(options)
            optGroups.push($optGroup)
        }

        this.setDataAdditioanlAction(optGroups)
    },
    setOpenStockAction: function (defaultAdditionalTargetId) {
        let options = []
        const fiersItemIsSelected = defaultAdditionalTargetId && defaultAdditionalTargetId > 0 ? '' : 'selected'

        options.push(`<option value='-1' disabled  ${fiersItemIsSelected}>Выберите акцию</option>`)
        for (let stock of StockManger.stockList) {
            const selected = defaultAdditionalTargetId == stock.Id ? 'selected' : ''

            options.push(`<option value='${stock.Id}'  ${selected}>${stock.Name}</option>`)
        }

        this.setDataAdditioanlAction(options)
    },
    setDataAdditioanlAction(options) {
        const query = '#push-additional-action'
        $(query).html(options)

        SumoSelects.PushAdditionalAction = $($(query)[0].sumo.reload()).parents('.SumoSelect')
        SumoSelects.PushAdditionalAction.hide()//для последующей анимации
        SumoSelects.PushAdditionalAction.show(this.animationSpeed)
    },
    getActionMsg: function () {
        const actionType = parseInt($('#push-actions option:selected').val())
        let targetId = -1

        switch (actionType) {
            case NotificationActionType.OpenCategory:
            case NotificationActionType.OpenProductInfo:
            case NotificationActionType.OpenStock:
                targetId = parseInt($('#push-additional-action option:selected').val())
                break
        }

        return {
            type: actionType,
            targetId: targetId
        }
    },
    isValidAction: function () {
        const action = this.getActionMsg()
        let isValid = false

        switch (action.type) {
            case NotificationActionType.NoAction:
            case NotificationActionType.OpenCashback:
            case NotificationActionType.OpenPartners:
                isValid = true
                break
            case NotificationActionType.OpenCategory:
            case NotificationActionType.OpenProductInfo:
            case NotificationActionType.OpenStock:
                isValid = action.targetId > 0
                break
        }

        return isValid
    }
}

var PushNotitifactionImage = {
    controlId: 'push-notification-image-preview',
    setPreviewImage: function (value) {
        const src = value ? value : '/images/default-image.jpg'

        this.setImage(src)
        this.toggleRemoveButton(value)
    },
    setImage: function (src) {
        $(`#${this.controlId}`).attr('src', src)
    },
    toggleRemoveButton: function (src) {
        const query = '#push-image-btn-remove'

        if (src) {
            $(query).removeClass('hide')
        } else {
            $(query).addClass('hide')
        }
    },
    removeImage: function () {
        $('#push-image-download').val('')
        PreviewDevice.setMessageImage()
        this.setPreviewImage()

    }
}

function addPreviewPushImage(input) {
    if (input.files && input.files[0]) {
        let reader = new FileReader();

        reader.onload = function (e) {

            PreviewDevice.setMessageImage(e.target.result)
            PushNotitifactionImage.setPreviewImage(e.target.result)
        }

        reader.readAsDataURL(input.files[0]);
    }
}

var DataCollectorNewMessage = {
    getData: function () {
        return {
            title: this.getTitleMsg(),
            body: this.getBodyMsg(),
            action: NotificationAction.getActionMsg(),
            imageUrl: null
        }
    },
    setDefaultValues: function (title, message) {
        $('#push-title').val(title || '')
        $('#push-body').val(message || '')
    },
    getTitleMsg: function () {
        return $('#push-title').val().trim()
    },
    getBodyMsg: function () {
        return $('#push-body').val().trim()
    },
    isValidMsg: function () {
        const data = this.getData()

        return !!(data.title && data.body)
    }
}

var PushDataHandler = {
    toggleBtnSave: function () {
        const $btn = $('#send-new-push-message')

        if (DataCollectorNewMessage.isValidMsg() &&
            NotificationAction.isValidAction())
            $btn.removeAttr('disabled')
        else
            $btn.attr('disabled', true)

    },
    sendNewPushMessage: function () {
        let loader = new Loader($("#pormotion-push-notification"))
        loader.start()
        const errMessage = 'При отправки сообщения, что-то пошло не так...'
        const pushNotification = DataCollectorNewMessage.getData()
        const saveFunc = (dataUploadImage) => {
            if (!dataUploadImage.Success) {
                showErrorMessage(dataUploadImage.ErrorMessage)
            }

            pushNotification.imageUrl = dataUploadImage.URL

            const successFunc = function (result, loader) {
                loader.stop();
                if (result.Success) {
                    setDefaultPushNotification()
                    showSuccessMessage('Push уведомления отправлены')
                } else {
                    showErrorMessage(result.ErrorMessage)
                }
            }

            $.post("/Admin/PushNotification", { pushNotification }, successCallBack(successFunc, loader))
        }

        let files = $("#push-image-download")[0].files;
        if (files.length == 0) {
            saveFunc({ Success: true, URL: null });

            return;
        }

        let dataImage = new FormData();
        for (var x = 0; x < files.length; x++) {
            dataImage.append("file" + x, files[x]);
        }

        $.ajax({
            type: 'POST',
            url: '/Admin/UploadImage',
            contentType: false,
            processData: false,
            data: dataImage,
            success: function (data) {
                saveFunc(data);
            },
            error: function () {
                loader.stop()
                showErrorMessage(errMessage)
            }
        })
    }
}

var HistoryNotification = {
    animationSpeedUp: 400,
    animataionSpeedDown: 600,
    pageNumber: 1,
    isLast: false,
    packageSize: 10,
    history: {},
    queryMainHestoryBlock: '.history-push-notitification',
    showHistory: function () {
        this.pageNumber = 1
        this.isLast = false
        let loader = new Loader(this.queryMainHestoryBlock);
        loader.start()
        this.toggleBlocksAnimation(true)
        setTimeout(() => {
            loader.stop()
            this.addHistoryListContainer()
            this.loadHistoryData()
        }, this.animationSpeedUp)
    },
    addHistoryListContainer: function () {
        const template = `
            <div class="push-history-wrapper" style="display: none">
                <span class="wizard-push-notification-item-header">История PUSH уведомлений</span>
                <div id="push-history-list-cotainer" class="push-history-list-cotainer"></div>
            </div>`

        $(this.queryMainHestoryBlock).append(template)
        $('.push-history-wrapper').fadeIn(this.animationSpeedUp, function () {
            $(this).css({ 'display': 'grid' })
        })
    },
    showEditNewMessage: function () {
        this.pageNumber = 1
        this.isLast = false
        this.toggleBlocksAnimation()
    },
    processingData: function (data) {
        for (let item of data) {
            item.Date = jsonToDate(item.Date)
        }

        return data
    },
    loadHistoryData: function () {
        const self = this
        let loader = new Loader(this.queryMainHestoryBlock);
        loader.start()

        let successFunc = function (result, loader) {
            if (result.Success) {
                let data = self.processingData(result.Data.HistoryMessages)
                self.isLast = result.Data.IsLast
                self.addHistoryToPage(data)
            } else {
                showErrorMessage(result.ErrorMessage);
            }

            loader.stop();
        }

        $.post("/Admin/LoadPushNotification", { pageNumber: this.pageNumber }, successCallBack(successFunc, loader));
    },
    addHistoryToPage: function (data) {
        if (!data || data.length == 0) {
            this.setEmptyInfo()
        } else {
            let messages = []
            const countPackageSizeToRender = 5

            for (const item of data) {
                this.history[item.Id] = item
                messages.push(this.renderHistoryItemMessage(item))

                if (countPackageSizeToRender == messages.length) {
                    this.addPackageHistoryToList(messages)
                    messages = []
                }
            }

            if (messages.length > 0)
                this.addPackageHistoryToList(messages)
        }

        this.toggleButtonShowMore(data.length)
    },
    renderHistoryItemMessage: function (data) {
        let date = toStringDateAndTime(new Date(data.Date))
        return `
            <div class="push-history-item" style="display: none">
                <i class="fas fa-paper-plane push-history-item-icon-start"></i>
                <span class="push-history-item-text">${data.Title}</span>
                <span class="push-history-item-text">${data.Body}</span>
                <img src="${data.ImageUrl ? data.ImageUrl : '/images/default-image.jpg'}">
                <span class="push-history-item-text">${date}</span>
                <button class="simple-text-button push-history-item-icon-edit" onClick="HistoryNotification.clonePushNotification(${data.Id})">
                    <i class="fal fa-edit"></i>
                </button>
            </div>`
    },
    clonePushNotification: function (id) {
        this.showEditNewMessage()
        setPushMessage(this.history[id])
    },
    addPackageHistoryToList: function (package) {
        $('#push-history-list-cotainer').append(package)
        $('#push-history-list-cotainer .push-history-item:hidden').fadeIn(200)
    },
    toggleButtonShowMore: function (loadedPackageSize = 0, callback) {
        const id = 'show-more-push-history-btn-container'

        $(`#${id}`).fadeOut(200, function () {
            $(this).remove()
            if (callback)
                callback()
        })

        if (!this.isLast) {
            const template = `
                <div id="${id}" class="show-more-push-history-btn-container">
                  <button class="simple-text-button" onClick="HistoryNotification.onShowMoreHistory()">
                    Показать ещё
                    <i class="fal fa-angle-double-right"></i>
                  </button>
                </div>`

            this.addPackageHistoryToList(template)

        }
    },
    onShowMoreHistory: function () {
        const callback = () => {
            this.pageNumber++;
            this.loadHistoryData()
        }
        this.toggleButtonShowMore(0, callback)
      
    },
    setEmptyInfo: function () {
        const template = `
            <div class="empty-list">
                <i class="fal fa-comment-alt-smile"></i>
                <span>Пока нет отправленых уведомлений</span>
            </div>`

        $('#push-history-list-cotainer').html(template)
    },
    toggleBlocksAnimation: function (isShowHistory = false) {
        if (isShowHistory) {
            $('.wizard-push-notification-item').slideUp(this.animationSpeedUp, () => {
                $('#container-collapse-edit-new-push-msg').removeClass('hide')
            })
            $('.wizard-push-notification').addClass('wizard-push-notification-collapse')
            $('#container-collapse-history-push-msg').addClass('hide')

        } else {
            $('.wizard-push-notification').removeClass('wizard-push-notification-collapse')
            $('.push-history-wrapper').remove()
            $('#container-collapse-history-push-msg').removeClass('hide')
            $('.wizard-push-notification-item').slideDown(this.animataionSpeedDown)
            $('#container-collapse-edit-new-push-msg').addClass('hide')

        }
    }
}