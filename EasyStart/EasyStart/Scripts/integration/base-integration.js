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

    hideSyncFrontpad()

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

function hideSyncFrontpad() {
    $(`#${frontPadIdsStore.syncClients}`).hide()
}

function showSyncFrontpad() {
    $(`#${frontPadIdsStore.syncClients}`).show()
}

function initWithoutIntegration() {
    const $integrationWrapper = $('.integration-values-wrapper')
    const template = '<div class="empty-container">Выберите интеграционную систему</div>'

    $integrationWrapper.html(template)
}

const frontPadIdsStore = {
    secret: 'frontpad-integration',
    statusNew: 'frontpad-integration-status-new',
    statusProcessed: 'frontpad-integration-status-processed',
    statusDelivery: 'frontpad-integration-status-delivery',
    statusDone: 'frontpad-integration-status-done',
    statusCancel: 'frontpad-integration-status-cancel',
    phoneCodeCountry: 'frontpad-integration-phone-code-country',
    useAutomaticDispatch: 'frontpad-integration-use-automatic-dispatch',
    syncClients: 'integration-sync-frontpad-wrapper',
    pointSaleDelivery: 'integration-point-sale-delivery',
    pointSaleTakeyourself: 'integration-point-sale-takeyourself',
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
        <div class="checkbox-item integration-frontpad-cbx-group">
            <input type="checkbox" id="${frontPadIdsStore.useAutomaticDispatch}">
            <label for="${frontPadIdsStore.useAutomaticDispatch}" class="label-for">Отправлять новый заказ автоматически</label>
        </div>        
        <div class="checkbox-item integration-frontpad-cbx-group">
            <input type="checkbox" id="${frontPadIdsStore.usePhoneMask}" checked>
            <label for="${frontPadIdsStore.usePhoneMask}" class="label-for">Использовать маску телефона +7(XXX)XXX-XX-XX</label>
        </div>
        <div class="group">
            <input required type="text" id="${frontPadIdsStore.phoneCodeCountry}" value="+7"/>
            <span class="bar"></span>
            <label>Код страны для номера телефона</label>
        </div>
        <div class="group">
            <input required readonly type="text" id="${frontPadIdsStore.statusNew}" value="1"/>
            <span class="bar"></span>
            <label>Статус новый (код api)</label>
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
            <input required readonly type="text" id="${frontPadIdsStore.statusDone}" value="10"/>
            <span class="bar"></span>
            <label>Статус выполнен (код api)</label>
        </div>
        <div class="group">
            <input required type="text" id="${frontPadIdsStore.statusCancel}" />
            <span class="bar"></span>
            <label>Статус списан или отменен (код api)</label>
        </div>
        <div class="group">
            <input required type="text" id="${frontPadIdsStore.pointSaleTakeyourself}" />
            <span class="bar"></span>
            <label>Точка продаж "Самовывоз" (код api)</label>
        </div>
        <div class="group">
            <input required type="text" id="${frontPadIdsStore.pointSaleDelivery}" />
            <span class="bar"></span>
            <label>Точка продаж "Доставка" (код api)</label>
        </div>
        <div class="group">
            <input required readonly type="text" value="${webHookUrl}"/>
            <span class="bar"></span>
            <label>Webhook url</label>
        </div>`)

    if (IntegerationSystemSetting && IntegerationSystemSetting.Type == IntegrationSystemType.frontPad) {
        $template.find(`#${frontPadIdsStore.secret}`).val(IntegerationSystemSetting.Secret)
        $template.find(`#${frontPadIdsStore.useAutomaticDispatch}`).prop('checked', IntegerationSystemSetting.UseAutomaticDispatch)

        const options = JSON.parse(IntegerationSystemSetting.Options)
        $template.find(`#${frontPadIdsStore.statusNew}`).val(options.statusNew || 1)
        $template.find(`#${frontPadIdsStore.statusProcessed}`).val(options.statusProcessed)
        $template.find(`#${frontPadIdsStore.statusDelivery}`).val(options.statusDelivery)
        $template.find(`#${frontPadIdsStore.statusDone}`).val(options.statusDone || 10)
        $template.find(`#${frontPadIdsStore.statusCancel}`).val(options.statusCancel)
        $template.find(`#${frontPadIdsStore.phoneCodeCountry}`).val(options.phoneCodeCountry)
        $template.find(`#${frontPadIdsStore.usePhoneMask}`).prop('checked', !(options.usePhoneMask === false))
        $template.find(`#${frontPadIdsStore.pointSaleTakeyourself}`).val(options.pointSaleTakeyourself || '')
        $template.find(`#${frontPadIdsStore.pointSaleDelivery}`).val(options.pointSaleDelivery || '')
        
    }

    $integrationWrapper.html($template)
    showSyncFrontpad()
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
        useAutomaticDispatch: false,
        options: JSON.stringify('')
    }
}

function getFrontPadSetting() {
    const secret = $(`#${frontPadIdsStore.secret}`).val().trim()
    const useAutomaticDispatch = $(`#${frontPadIdsStore.useAutomaticDispatch}`).is(':checked')
    const statusNew = $(`#${frontPadIdsStore.statusNew}`).val().trim()
    const statusProcessed = $(`#${frontPadIdsStore.statusProcessed}`).val().trim()
    const statusDelivery = $(`#${frontPadIdsStore.statusDelivery}`).val().trim()
    const statusDone = $(`#${frontPadIdsStore.statusDone}`).val().trim()
    const statusCancel = $(`#${frontPadIdsStore.statusCancel}`).val().trim()
    let pointSaleTakeyourself = $(`#${frontPadIdsStore.pointSaleTakeyourself}`).val().trim()
    let pointSaleDelivery = $(`#${frontPadIdsStore.pointSaleDelivery}`).val().trim()
    pointSaleTakeyourself = parseInt(pointSaleTakeyourself)
    pointSaleDelivery = parseInt(pointSaleDelivery)
    pointSaleTakeyourself = Number.isNaN(pointSaleTakeyourself) ? 0 : pointSaleTakeyourself
    pointSaleDelivery = Number.isNaN(pointSaleDelivery) ? 0 : pointSaleDelivery

    const options = {
        statusNew: parseInt(statusNew),
        statusProcessed: parseInt(statusProcessed),
        statusDelivery: parseInt(statusDelivery),
        statusDone: parseInt(statusDone),
        statusCancel: parseInt(statusCancel),
        pointSaleTakeyourself: pointSaleTakeyourself,
        pointSaleDelivery: pointSaleDelivery,
        phoneCodeCountry: $(`#${frontPadIdsStore.phoneCodeCountry}`).val().trim(),
        usePhoneMask: $(`#${frontPadIdsStore.usePhoneMask}`).is(':checked'),
    }

    if (!secret
        || !options.phoneCodeCountry
        || options.statusNew === 0 || Number.isNaN(options.statusNew)
        || options.statusProcessed === 0 || Number.isNaN(options.statusProcessed)
        || options.statusDelivery === 0 || Number.isNaN(options.statusDelivery)
        || options.statusDone === 0 || Number.isNaN(options.statusDone)
        || options.statusCancel === 0 || Number.isNaN(options.statusCancel)) {
        const message = 'Не все поля заполненны корректно'
        showErrorMessage(message)
        throw new Error(message)
    }

    return {
        type: IntegrationSystemType.frontPad,
        secret,
        useAutomaticDispatch,
        options: JSON.stringify(options)
    }
}

function getIikoSetting() {
    const secret = $('#iiko-integration').val().trim()
    const useAutomaticDispatch = $(`#${frontPadIdsStore.useAutomaticDispatch}`).is(':checked')
    const options = ''

    if (!secret) {
        const message = 'Не все поля заполненны'
        showErrorMessage(message)
        throw new Error(message)
    }

    return {
        type: IntegrationSystemType.iiko,
        secret,
        useAutomaticDispatch,
        options: JSON.stringify(options)
    }
}

function getTypeIntegrationSystem() {
    return parseInt($('#integration-type option:selected').val())
}

function syncFrontpadClients() {
    const $input = $(`#${frontPadIdsStore.syncClients} input[type=file]`)
    const input = $input[0]

    if (input.files && input.files[0]) {
        const file = input.files[0]
        let reader = new FileReader();

        reader.onload = function (e) {
            const loader = new Loader($('.integration-values-wrapper'))
            loader.start()

            const successMsg = 'Баллы синхронизированны'
            const errMsg = 'При синхронизации что-то пошло не так...'

            const formData = new FormData();
            formData.append('file', file, file.name);

            $.ajax({
                type: 'POST',
                url: '/IntegrationSystem/SyncFrontpadClientsCashback',
                contentType: false,
                processData: false,
                data: formData,
                success: function (data) {
                    loader.stop()
                    if (data.Success) {
                        $input.val('')
                        showSuccessMessage(successMsg)
                    } else
                        showErrorMessage(data.ErrorMessage)
                },
                error: function () {
                    loader.stop()
                    showErrorMessage(errMsg)
                }
            });
        }

        reader.readAsDataURL(file);
    } else
        return
}