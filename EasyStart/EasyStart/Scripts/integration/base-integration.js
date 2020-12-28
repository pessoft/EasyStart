$(document).ready(function() {
    bindChangedIntegragionSelector()
})

const IntegrationSystemType = {
    withoutIntegration: 0,
    frontPad: 1,
    iiko:2
}

function bindChangedIntegragionSelector() {
    const $integrationSystems = $('#integration-type')
    $integrationSystems.unbind('change')
    $integrationSystems.bind('change', changeIntegrationSystemHandler)
}

function changeIntegrationSystemHandler() {
    const integrationSystemType = parseInt($('#integration-type option:selected').val())

    switch(integrationSystemType) {
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

function initFrontPadIntegration() {
    const $integrationWrapper = $('.integration-values-wrapper')
    const inputId = 'frontpad-integration'
    const template = `
        <div class="group">
            <input required type="text" id="${inputId}" />
            <span class="bar"></span>
            <label>Секрет из FrontPad</label>
        </div>`

    $integrationWrapper.html(template)
}

function initIikoIntegration() {
    const $integrationWrapper = $('.integration-values-wrapper')
    const inputId = 'iiko-integration'
    const template = `
        <div class="group">
            <input required type="text" id="${inputId}" />
            <span class="bar"></span>
            <label>Секрет из Iiko</label>
        </div>`

    $integrationWrapper.html(template)
}

function getIntegrationSystemSetting() {
    const systemType = parseInt($('#integration-type option:selected').val())
    let secret = ''

    switch(systemType) {
        case IntegrationSystemType.frontPad:
            secret = $('#frontpad-integration').val()
            break
        case IntegrationSystemType.iiko:
            secret = $('#iiko-integration').val()
            break
    }

    return {
        systemType,
        secret
    }
}

function saveIntegrationSystemSetting() {
    const integrationSystemSetting = getIntegrationSystemSetting()

    fetchSaveIntegraionSystemSetting(integrationSystemSetting)
}

function fetchSaveIntegraionSystemSetting(setting) {
    const loader = new Loader($('.integration-values-wrapper'))
    loader.start()

    let callback = (data, loader) => {
        loader.stop()

        if (data.Success)
            showSuccessMessage('Настройки системы интегации сохранены')
        else {
            console.log(data.ErrorMessage)
            showErrorMessage('Во время сохранения насройки что-то пошло не так. Пожалуйста поробуйте еще раз.')
        }
    }

    $.post("/IntegrationSystem/Save",
        setting,
        successCallBack(callback, loader)).catch(function () {
            loader.stop()
        })
}