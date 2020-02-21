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
    PreviewDevice.setDefaultValues()
    NotificationAction.setDefaultAction()
    PushNotitifactionImage.setPreviewImage()
    PushDataHandler.toggleBtnSave()
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
    setDefaultValues: function () {
        this.setMessageTitle()
        this.setMessageBody()
        this.setMessageImage()
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
    setDefaultAction: function () {
        const query = '#push-actions'

        $(query).val(NotificationActionType.NoAction)
        this.onChangeAction(query)
    },
    onChangeAction: function (e) {
        const actionType = parseInt($(e).children('option:selected').val())

        this.toggleAdditionalSelectControl(actionType)
    },
    toggleAdditionalSelectControl: function (actionType) {
        switch (actionType) {
            case NotificationActionType.NoAction:
            case NotificationActionType.OpenCashback:
            case NotificationActionType.OpenPartners:
                SumoSelects.PushAdditionalAction.hide(this.animationSpeed)
                break
            case NotificationActionType.OpenCategory:
                this.setOpenCategoryAction()
                break
            case NotificationActionType.OpenProductInfo:
                this.setOpenProductAction()
                break
            case NotificationActionType.OpenStock:
                this.setOpenStockAction()
                break
        }
    },
    setOpenCategoryAction: function () {
        let options = []

        options.push(`<option value='-1' disabled selected>Выберите категорию</option>`)
        for (let id in CategoryDictionary) {
            const categoryName = CategoryDictionary[id]

            options.push(`<option value='${id}'>${categoryName}</option>`)
        }

        this.setDataAdditioanlAction(options)
    },
    setOpenProductAction: function () {
        let optGroups = []
        optGroups.push(`<option value='-1' disabled selected>Выберите блюдо</option>`)
        for (let id in CategoryDictionary) {
            const categoryName = CategoryDictionary[id]
            const $optGroup = $(`<optgroup label='${categoryName}'></optgroup>`)
            const products = ProductsForPromotion[id]
            let options = []

            for (product of products) {
                options.push(`<option value='${product.Id}'>${product.Name}</option>`)
            }

            $optGroup.html(options)
            optGroups.push($optGroup)
        }

        this.setDataAdditioanlAction(optGroups)
    },
    setOpenStockAction: function () {
        let options = []

        options.push(`<option value='-1' disabled selected>Выберите акцию</option>`)
        for (let stock of StockManger.stockList) {
            options.push(`<option value='${stock.Id}'>${stock.Name}</option>`)
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
                    //setDefaultPushNotification()
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