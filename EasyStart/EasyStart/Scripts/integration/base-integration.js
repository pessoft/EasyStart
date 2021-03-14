$(document).ready(function () {
    bindChangedIntegragionSelector()
    initInitIntegrationtSystemSetting()
})

const IntegrationSystemType = {
    withoutIntegration: 0,
    frontPad: 1,
    iiko: 2
}

function bindChangedIntegragionSelector() {
    const $integrationSystems = $('#integration-type')
    $integrationSystems.unbind('change')
    $integrationSystems.bind('change', changeIntegrationSystemHandler)
}

function changeIntegrationSystemHandler() {
    const integrationSystemType = parseInt($('#integration-type option:selected').val())

    switch (integrationSystemType) {
        case IntegrationSystemType.withoutIntegration:
            initWithoutIntegration()
            break
        case IntegrationSystemType.frontPad:
            initFrontPadIntegration()
            break
        case IntegrationSystemType.iiko:
            initIikoIntegration()
            break
    }
}

function initWithoutIntegration() {
    const $integrationWrapper = $('.integration-values-wrapper')
    const template = '<div class="empty-container">Выберите интеграционную систему</div>'

    $integrationWrapper.html(template)
}

const frontPadIdsStore = {
    secret: 'frontpad-integration',
    statusProcessed: 'frontpad-integration-status-processed',
    statusDelivery: 'frontpad-integration-status-delivery',
    statusCancel: 'frontpad-integration-status-cancel',
}

function initFrontPadIntegration() {
    const $integrationWrapper = $('.integration-values-wrapper')
    const webHookUrl = `${window.location.origin}/api/frontpad/changestatus`
    const $template = $(`
        <div class="group">
            <input required type="text" id="${frontPadIdsStore.secret}" />
            <span class="bar"></span>
            <label>Секрет из FrontPad</label>
        </div>
        <div class="group">
            <input required type="text" id="${frontPadIdsStore.statusProcessed}" />
            <span class="bar"></span>
            <label>Статус в производстве (код api)</label>
        </div>
        <div class="group">
            <input required type="text" id="${frontPadIdsStore.statusDelivery}" />
            <span class="bar"></span>
            <label>Статус в пути (код api)</label>
        </div>
        <div class="group">
            <input required type="text" id="${frontPadIdsStore.statusCancel}" />
            <span class="bar"></span>
            <label>Статус списан или отменен (код api)</label>
        </div>
        <div class="group">
            <input required readonly type="text" value="${webHookUrl}"/>
            <span class="bar"></span>
            <label>Webhook url</label>
        </div>`)

    if (IntegerationSystemSetting && IntegerationSystemSetting.Type == IntegrationSystemType.frontPad) {
        $template.find(`#${frontPadIdsStore.secret}`).val(IntegerationSystemSetting.Secret)

        const options = JSON.parse(IntegerationSystemSetting.Options)
        $template.find(`#${frontPadIdsStore.statusProcessed}`).val(options.statusProcessed)
        $template.find(`#${frontPadIdsStore.statusDelivery}`).val(options.statusDelivery)
        $template.find(`#${frontPadIdsStore.statusCancel}`).val(options.statusCancel)
    }
    
    $integrationWrapper.html($template)
}

function initIikoIntegration() {
    const secret = IntegerationSystemSetting && IntegerationSystemSetting.Type == IntegrationSystemType.iiko ?
        IntegerationSystemSetting.Secret : ''
    const $integrationWrapper = $('.integration-values-wrapper')
    const inputId = 'iiko-integration'
    const $template = $(`
        <div class="group">
            <input required type="text" id="${inputId}" />
            <span class="bar"></span>
            <label>Секрет из Iiko</label>
        </div>`)

    $template.find(`#${inputId}`).val(secret)
    $integrationWrapper.html($template)
}

function getIntegrationSystemSetting() {
    const type = getTypeIntegrationSystem()
    let setting = getDefaultSetting()

    switch (type) {
        case IntegrationSystemType.frontPad:
            setting = getFrontPadSetting()
            break
        case IntegrationSystemType.iiko:
            setting = getIikoSetting()
            break
    }

    return setting
}

function saveIntegrationSystemSetting() {
    try {
        const integrationSystemSetting = getIntegrationSystemSetting()
        const loader = new Loader($('.integration-values-wrapper'))
        loader.start()

        let callback = (result, loader) => {
            loader.stop()

            if (result.Success) {
                IntegerationSystemSetting = result.Data
                showSuccessMessage('Настройки системы интегации сохранены')
            } else {
                console.log(result.ErrorMessage)
                showErrorMessage('Во время сохранения насройки что-то пошло не так. Пожалуйста поробуйте еще раз.')
            }
        }

        $.post("/IntegrationSystem/Save",
            integrationSystemSetting,
            successCallBack(callback, loader)).catch(function () {
                loader.stop()
            })
    } catch (ex) {
        console.log(ex)
    }
}

var IntegerationSystemSetting = null
function loadIntegraionSystemSetting(successAction) {
    const loader = new Loader($('.integration-values-wrapper'))
    loader.start()

    let callback = (result, loader) => {
        loader.stop()

        if (result.Success) {
            IntegerationSystemSetting = result.Data
            successAction && successAction()
        } else {
            console.log(result.ErrorMessage)
        }
    }

    $.get("/IntegrationSystem/Get",
        null,
        successCallBack(callback, loader)).catch(function () {
            loader.stop()
        })
}

var isInitIntegrationtSystem = false
function initInitIntegrationtSystemSetting() {
    if (isInitIntegrationtSystem)
        return

    const callback = () => {
        setIntegrationSystemSetting()
        isInitIntegrationtSystem = true
    }
    loadIntegraionSystemSetting(callback)
}

function setIntegrationSystemSetting() {
    if (!IntegerationSystemSetting)
        return

    setIntegrationType()
    changeIntegrationSystemHandler()
}

function setIntegrationType() {
    $('#integration-type').val(IntegerationSystemSetting.Type)
}

function sendOrderToIntegrationSystem(orderId) {
    const $loader = $(OrderDetailsQSelector.SendToIntegtationSystemLoader)
    $loader.show()
    const loader = new Loader($loader)
    loader.start()

    const $sendBtn = $(OrderDetailsQSelector.SendToIntegtationSystem)
    $sendBtn.hide()

    const $intergrationOrderNumber = $(OrderDetailsQSelector.IntegrationOrderNumber)
    $intergrationOrderNumber.hide()

    let callback = (result) => {
        loader.stop()
        $loader.hide()

        if (result.Success) {
            showSuccessMessage('Заказ отправлен')
            $intergrationOrderNumber.html(`#${result.Data}`)
            $intergrationOrderNumber.show()

            const order = Orders.find(p => p.Id == orderId)
            order.IntegrationOrderNumber = result.Data
            order.IsSendToIntegrationSystem = true
        } else {
            showErrorMessage('Не отправлено')
            $sendBtn.show()
        }
    }

    $.post("/IntegrationSystem/SendOrder", { orderId }, successCallBack(callback))
}

function getDefaultSetting() {
    return {
        type: IntegrationSystemType.withoutIntegration,
        secret: '',
        options: JSON.stringify('')
    }
}

function getFrontPadSetting() {
    const secret = $(`#${frontPadIdsStore.secret}`).val().trim()
    const statusProcessed = $(`#${frontPadIdsStore.statusProcessed}`).val().trim()
    const statusDelivery = $(`#${frontPadIdsStore.statusDelivery}`).val().trim()
    const statusCancel = $(`#${frontPadIdsStore.statusCancel}`).val().trim()

    const options = {
        statusProcessed: parseInt(statusProcessed),
        statusDelivery: parseInt(statusDelivery),
        statusCancel: parseInt(statusCancel),
    }

    if (!secret
        || options.statusProcessed === 0 || Number.isNaN(options.statusProcessed)
        || options.statusDelivery === 0 || Number.isNaN(options.statusDelivery)
        || options.statusCancel === 0 || Number.isNaN(options.statusCancel)) {
        const message = 'Не все поля заполненны корректно'
        showErrorMessage(message)
        throw new Error(message)
    }

    return {
        type: IntegrationSystemType.frontPad,
        secret,
        options: JSON.stringify(options)
    }
}

function getIikoSetting() {
    const secret = $('#iiko-integration').val().trim()
    const options = ''

    if (!secret) {
        const message = 'Не все поля заполненны'
        showErrorMessage(message)
        throw new Error(message)
    }

    return {
        type: IntegrationSystemType.iiko,
        secret,
        options: JSON.stringify(options)
    }
}

function getTypeIntegrationSystem() {
    return parseInt($('#integration-type option:selected').val())
}