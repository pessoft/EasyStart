const ProductAdditionalInfoType = {
    Custom: 0,
    Grams: 1,
    Milliliters: 2,
    Liters: 3
}

const ProductAdditionalInfoTypeShortName = {
    0: '',
    1: 'гр.',
    2: 'мл',
    3: 'л'
}

function onChangeProductAdditaonalInfoType(e) {
    const $e = $(e)
    const value = parseInt($e.val())
    const $btnFunctionAdditionnalInfo = $('#btn-function-product-additional-info')
    const $btnFunctioOptionsInfo = $('#btn-product-options')
    const $inputAdditionalInfo = $('#product-additional-info')
    const type = value == ProductAdditionalInfoType.Custom ? 'text' : 'number'

    $inputAdditionalInfo.attr('type', type)
    $btnFunctionAdditionnalInfo.attr('disabled', value == ProductAdditionalInfoType.Custom)
    $btnFunctioOptionsInfo.attr('disabled', value == ProductAdditionalInfoType.Custom)
}

function openFunctionAdditionalInfoDialog(event) {
    event.stopPropagation()

    renderAdditionalOptionFromProduct()
    Dialog.showModal($('#functionAdditionalInfoDialog'))
}

function openAdditionalOptionsDialog(event) {
    event.stopPropagation()

    Dialog.showModal($('#productAdditionalOptionsDialog'))
}

function openCreateAdditionalOptionsDialog(event) {
    event.stopPropagation()
    cleanAdditinOption()

    Dialog.showModal($('#createFunctionAdditionalInfoDialog'))
}

function addRowAdditionalOptionsInDialog(event) {
    event.stopPropagation()

    const dialogId = 'createFunctionAdditionalInfoDialog'
    const $containerProps = $(`#${dialogId} .create-functions-additional`)
    const productAdditionalInfoTypeSelected = parseInt($('#product-additional-info-type option:selected').val())
    const tmpId = generateRandomString()
    const isEmptyContainer = $containerProps.find('.empty-container').length != 0
    const templateRow = `
        <div class="additional-option-item" id="${tmpId}">
            <div class="additional-option-name">
                <input type="text" class="default-color default-style-input" placeholder="Название">
            </div>

            <div class="additional-option-value">
                <input type="number" min="0" class="default-color default-style-input">
                <span>${ProductAdditionalInfoTypeShortName[productAdditionalInfoTypeSelected]}</span>
            </div>
            
            <div class="additional-option-price">
                <input type="number" min="0" class="default-color default-style-input">
                <span>руб.</span>
            </div>
            
            <div class="additional-option-default-check">
                <input id="${tmpId}-default" type="radio" name="additional-option-default-check" ${isEmptyContainer ? 'checked' : ''}>
                <label for="${tmpId}-default">по умлочанию</label>
            </div>
            <button class="remove-options-btn" onclick="removeRowAdditionalOptionsFromDialog(event, this)">
                <i class="fal fa-trash-alt"></i>
            </button>
        </div>
    `

    if (isEmptyContainer)
        $containerProps.html(templateRow)
    else
        $containerProps.append(templateRow)
}

function removeRowAdditionalOptionsFromDialog(event, e) {
    event.stopPropagation()

    const $e = $(e)
    const $row = $e.parents('.additional-option-item')

    $row.remove()

    const dialogId = 'createFunctionAdditionalInfoDialog'
    const $containerProps = $(`#${dialogId} .create-functions-additional`)

    if ($containerProps.children().length == 0)
        renderEmptyInfoAdditionalOptionInDialog()
    else {
        const $radioDefOptions = $containerProps.find('.additional-option-item [name=additional-option-default-check]')
        const checkedDefOption = $radioDefOptions.is(':checked')

        if (!checkedDefOption) {
            $($radioDefOptions[0]).attr('checked', true)
        }
    }
}

function renderEmptyInfoAdditionalOptionInDialog() {
    const template = `<div class="empty-container">Добавте значения свойства</div>`

    const dialogId = 'createFunctionAdditionalInfoDialog'
    const $containerProps = $(`#${dialogId} .create-functions-additional`)

    $containerProps.html(template)
}

function doneAdditionalOption() {
    const dialogId = 'createFunctionAdditionalInfoDialog'
    const additionalOptionName = $('#function-additional-info-name-block').val().trim()
    const additionalOptionId = $(`#${dialogId}`).attr('additional-option-id')

    if (!additionalOptionName) {
        showInfoMessage('Укажите название свойства')

        return
    }

    const $containerProps = $(`#${dialogId} .create-functions-additional`)
    const isEmptyContainer = $containerProps.find('.empty-container').length != 0

    if (isEmptyContainer) {
        showInfoMessage('Добавте значения свойства')

        return
    }

    /**
     * 
     * item = {
     *  name: string,
     *  additionalInfo: float
     *  price: float,
     *  isDefault: bool
     * }
     * */
    let additionOptionItems = []
    $containerProps.find('.additional-option-item').each(function () {
        const $e = $(this)
        const name = $e.find('.additional-option-name input').val().trim()
        const additionalInfo = parseFloat($e.find('.additional-option-value input').val())
        const price = parseFloat($e.find('.additional-option-price input').val())
        const isDefault = $e.find('.additional-option-default-check input').is(':checked')

        if (!name ||
            (Number.isNaN(additionalInfo) || additionalInfo < 0) ||
            (Number.isNaN(price) || price < 0) ||
            (isDefault == null || typeof (isDefault) === 'undefined')) {
            showInfoMessage('Заполните все поля значений корректными данными')
            additionOptionItems = []

            return
        }

        additionOptionItems.push({ name, additionalInfo, price, isDefault })
    })

    let additionalOption = {
        id: additionalOptionId,
        name: additionalOptionName,
        items: additionOptionItems
    }

    const callbackAfterSaveAdditionalOption = () => {
        closeAdditionalOption()
        renderAdditionalOptionFromProduct()
    }

    saveAdditionalOption(additionalOption, callbackAfterSaveAdditionalOption)
}

function closeAdditionalOption() {
    cleanAdditinOption()
    Dialog.close('#createFunctionAdditionalInfoDialog')
}

function cleanAdditinOption() {
    $('#function-additional-info-name-block').val('')
    $('#createFunctionAdditionalInfoDialog').attr('additional-option-id', 0)
    renderEmptyInfoAdditionalOptionInDialog()
}

function saveAdditionalOption(additionalOption, callbackAfterSave) {
    const loader = new Loader($("#createFunctionAdditionalInfoDialog"))
    loader.start()

    let callback = (data, loader) => {
        if (data.Success) {
            ProductAdditionalOptions.push(data.Data.Id)
            DataProduct.AdditionalOptions[data.Data.Id] = data.Data
            callbackAfterSave()
        }

        loader.stop()
    }

    $.post("/Admin/SaveProductAdditionalOption",
        additionalOption,
        successCallBack(callback, loader)).catch(function () {
            loader.stop()
        })
}

function renderAdditionalOptionFromProduct() {
    const templateOption = []
    const $containerForOptions = $('#functionAdditionalInfoDialog .functions-additional')
    const emptyTemplate = '<div class="empty-container">Добавте дополнительные опции</div>'

    for (const additionalOptionId of ProductAdditionalOptions) {
        const option = DataProduct.AdditionalOptions[additionalOptionId]
        const template = `
            <div class="funcitons-additional-item" id="${option.Id}">
                <div>${option.Name}</div>
                <div>
                    <button class="edit-option-setting-btn">
                        <i class="fal fa-edit"></i>
                    </button>
                </div>
                <div>
                    <button class="remove-option-setting-btn">
                        <i class="fal fa-trash-alt"></i>
                    </button>
                </div>
            </div>
        `

        templateOption.push(template)
    }

    $containerForOptions.html(templateOption.length ? templateOption : emptyTemplate)
}

