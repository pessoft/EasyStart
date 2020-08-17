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

    toggleDisabledExistAdditionalOptionBtn()
    Dialog.showModal($('#functionAdditionalInfoDialog'))
}

function toggleDisabledExistAdditionalOptionBtn() {
    $('#btn-function-additional-select').attr('disabled', Object.keys(DataProduct.AdditionalOptions) == 0)
}

function openFunctionAdditionalExistingOptionDialog(event) {
    event.stopPropagation()

    renderAdditionalExistingOptions()

    Dialog.showModal($('#functionAdditionalExistingOptionsDialog'))
}

function openAdditionalOptionsDialog(event) {
    event.stopPropagation()

    toggleDisabledExistAdditionalFillingBtn()
    Dialog.showModal($('#productAdditionalFillingsDialog'))
}

function openCreateAdditionalOptionsDialog(event) {
    event.stopPropagation()
    cleanAdditinOption()

    Dialog.showModal($('#createFunctionAdditionalInfoDialog'))
}

function editCreateAdditionalOptionsDialog(event, additionalOptionId) {
    event.stopPropagation()
    cleanAdditinOption()
    addDataAdditionalOptionsInDialog(DataProduct.AdditionalOptions[additionalOptionId])

    Dialog.showModal($('#createFunctionAdditionalInfoDialog'))
}

function addDataAdditionalOptionsInDialog(additionalOption) {
    const dialogId = 'createFunctionAdditionalInfoDialog'
    const $containerProps = $(`#${dialogId} .create-functions-additional`)
    const productAdditionalInfoTypeSelected = parseInt($('#product-additional-info-type option:selected').val())
    const templateRows = []

    $('#function-additional-info-name-block').val(additionalOption.Name)
    $(`#${dialogId}`).attr('additional-option-id', additionalOption.Id)

    for (const row of additionalOption.Items) {
        
        const templateRow = `
        <div class="additional-option-item" id="${row.Id}">
            <div class="additional-option-name">
                <input type="text" class="default-color default-style-input" placeholder="Название" value="${row.Name}">
            </div>

            <div class="additional-option-value">
                <input type="number" min="0" class="default-color default-style-input" value="${row.AdditionalInfo}">
                <span>${ProductAdditionalInfoTypeShortName[productAdditionalInfoTypeSelected]}</span>
            </div>
            
            <div class="additional-option-price">
                <input type="number" min="0" class="default-color default-style-input" value="${row.Price}">
                <span>руб.</span>
            </div>
            
            <div class="additional-option-default-check">
                <input id="${row.Id}-default" type="radio" name="additional-option-default-check" ${row.IsDefault ? 'checked' : ''}>
                <label for="${row.Id}-default">по умлочанию</label>
            </div>
            <button class="remove-options-btn" onclick="removeRowAdditionalOptionsFromDialog(event, this)">
                <i class="fal fa-trash-alt"></i>
            </button>
        </div>
    `

        templateRows.push(templateRow)
    }

    $containerProps.html(templateRows)
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
        toggleDisabledExistAdditionalOptionBtn()
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

    toggleDisabledExistAdditionalOptionBtn()
    renderEmptyInfoAdditionalOptionInDialog()
}

function saveAdditionalOption(additionalOption, callbackAfterSave) {
    const loader = new Loader($("#createFunctionAdditionalInfoDialog"))
    loader.start()

    let callback = (data, loader) => {
        if (data.Success) {
            if (ProductAdditionalOptions.findIndex(p => p == data.Data.Id) == -1)
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
                    <button class="edit-option-setting-btn" onclick="editCreateAdditionalOptionsDialog(event, ${option.Id})">
                        <i class="fal fa-edit"></i>
                    </button>
                </div>
                <div>
                    <button class="remove-option-setting-btn" onclick="removeAdditionalOptionFromProduct(event, ${option.Id})">
                        <i class="fal fa-trash-alt"></i>
                    </button>
                </div>
            </div>
        `

        templateOption.push(template)
    }

    $containerForOptions.html(templateOption.length ? templateOption : emptyTemplate)
}

function removeAdditionalOptionFromProduct(event, additionalOptionId) {
    event.stopPropagation()

    const i = ProductAdditionalOptions.findIndex(p => p == additionalOptionId)

    if (i != -1) {
        ProductAdditionalOptions.splice(i, 1)

        renderAdditionalOptionFromProduct()
    }
}

function changeOrderProductAdditionalOption() {
    let newProductAdditionalOptions = []
    $('#functionAdditionalInfoDialog .functions-additional .funcitons-additional-item').each(function () {
        const id = parseInt($(this).attr('id'))

        newProductAdditionalOptions.push(id)
    })

    ProductAdditionalOptions = newProductAdditionalOptions
}

function renderAdditionalExistingOptions() {
    const options = []
    for (const optionId in DataProduct.AdditionalOptions) {
        const additionOption = DataProduct.AdditionalOptions[optionId]
        const isSelecetedCurrentOption = ProductAdditionalOptions.includes(additionOption.Id)
        const optionItemTemplate = getAdditionalOptionExistTempalte(additionOption, isSelecetedCurrentOption)

        options.push(optionItemTemplate)
    }

    $('#functionAdditionalExistingOptionsDialog .functions-additional').html(options)
}

function getAdditionalOptionExistTempalte(option, isSelecetedOption) {
    return `
        <div class="funcitons-additional-item-exist">
            <div class="checkbox-item checkbox-item-left">
                <input type="checkbox" value="${option.Id}" ${isSelecetedOption ? 'checked' : ''}/>
            </div>
            <div>${option.Name}</div>
            <div>
                <button class="edit-option-setting-btn" onclick="editCreateAdditionalOptionsDialog(event, ${option.Id})">
                    <i class="fal fa-edit"></i>
                </button>
            </div>
            <div>
                <button class="remove-option-setting-btn" onclick="removeExistingOption(event, ${option.Id})">
                    <i class="fal fa-trash-alt"></i>
                </button>
            </div>
        </div>
    `
}

function complitedSelectionExistingOptions() {
    ProductAdditionalOptions = []

    $('#functionAdditionalExistingOptionsDialog .functions-additional input[type=checkbox]:checked').each(function () {
        const id = parseInt($(this).val())
        ProductAdditionalOptions.push(id)
    })

    renderAdditionalOptionFromProduct()
    Dialog.close('#functionAdditionalExistingOptionsDialog')
}

function removeExistingOption(event, id) {
    event.stopPropagation()

    let callback = function () {
        const loader = new Loader('#functionAdditionalExistingOptionsDialog .dialog-body-function-additional-wrapper')
        loader.start()
        let successFunc = function (result, loader) {
            loader.stop()
            if (result.Success) {
                delete (DataProduct.AdditionalOptions[id])
                renderAdditionalExistingOptions()

                const i = ProductAdditionalOptions.findIndex(p => p == id)
                if (i != -1) {
                    ProductAdditionalOptions.splice(i, 1)
                    
                    renderAdditionalOptionFromProduct()
                }

                if (!Object.keys(DataProduct.AdditionalOptions).length) {
                    Dialog.close('#functionAdditionalExistingOptionsDialog')
                    toggleDisabledExistAdditionalOptionBtn()
                }
            } else {
                showErrorMessage(result.ErrorMessage)
            }
        }
        $.post("/Admin/RemoveProductAdditionalOption", { id: id }, successCallBack(successFunc, loader))
    }

    deleteConfirmation(callback)
}

function showEditAdditionalFillingDialog(event, id = -1) {
    event.stopPropagation()
    cleanAdditionalFillingInputs()

    if (id != -1)
        editAdditionalFilling(id)

    Dialog.showModal($('#addProductAdditionalFillingDialog'))
}

function cleanAdditionalFillingInputs() {
    $('#additional-filling-id').val(-1)
    $('#additional-filling-name').val('')
    $('#additional-filling-price').val('')
}

function editAdditionalFilling(id) {
    const data = DataProduct.AdditionalFillings[id]

    $('#additional-filling-id').val(data.Id)
    $('#additional-filling-name').val(data.Name)
    $('#additional-filling-price').val(data.Price)
}

function doneEditAdditionalFilling(event) {
    event.stopPropagation()

    const additionalFilling = {
        Id: parseInt($('#additional-filling-id').val()),
        Name: $('#additional-filling-name').val().trim(),
        Price: parseFloat($('#additional-filling-price').val()),
    }

    if (!additionalFilling.Name ||
        (Number.isNaN(additionalFilling.Price) || additionalFilling.Price < 0)) {
        showInfoMessage('Заполните все поля корректными данными')

        return
    }

    saveAdditionalFilling(additionalFilling)
}

function saveAdditionalFilling(additionalFilling) {
    const loader = new Loader($('#addProductAdditionalFillingDialog'))
    loader.start()

    let callback = (data, loader) => {
        loader.stop()

        if (data.Success) {
            if (ProductAdditionalFillings.findIndex(p => p == data.Data.Id) == -1)
                ProductAdditionalFillings.push(data.Data.Id)

            DataProduct.AdditionalFillings[data.Data.Id] = data.Data

            Dialog.close('#addProductAdditionalFillingDialog')
            toggleDisabledExistAdditionalFillingBtn()
            renderAdditionalFillingFromProduct()
        }
    }

    $.post("/Admin/SaveAdditionalFilling",
        additionalFilling,
        successCallBack(callback, loader)).catch(function () {
            loader.stop()
        })
}

function toggleDisabledExistAdditionalFillingBtn() {
    $('#btn-additional-fillings-select').attr('disabled', Object.keys(DataProduct.AdditionalFillings) == 0)
}

function renderAdditionalFillingFromProduct() {
    const templateOption = []
    const $containerForOptions = $('#productAdditionalFillingsDialog .additional-filling-list')
    const emptyTemplate = '<div class="empty-container">Добавте дополнительные опции</div>'

    for (const additionalFillingId of ProductAdditionalFillings) {
        const option = DataProduct.AdditionalFillings[additionalFillingId]
        const template = `
            <div class="funcitons-additional-item" id="${option.Id}">
                <div>${option.Name}</div>
                <div>
                    <button class="edit-option-setting-btn" onclick="showEditAdditionalFillingDialog(event, ${option.Id})">
                        <i class="fal fa-edit"></i>
                    </button>
                </div>
                <div>
                    <button class="remove-option-setting-btn" onclick="removeAdditionalFillingFromProduct(event, ${option.Id})">
                        <i class="fal fa-trash-alt"></i>
                    </button>
                </div>
            </div>
        `

        templateOption.push(template)
    }

    $containerForOptions.html(templateOption.length ? templateOption : emptyTemplate)
}

function removeAdditionalFillingFromProduct(event, additionalFillingId) {
    event.stopPropagation()

    const i = ProductAdditionalFillings.findIndex(p => p == additionalFillingId)

    if (i != -1) {
        ProductAdditionalFillings.splice(i, 1)

        renderAdditionalFillingFromProduct()
    }
}

function changeOrderProductAdditionalFilling() {
    let newProductAdditionalFillings = []
    $('#productAdditionalFillingsDialog .additional-filling-list .funcitons-additional-item').each(function () {
        const id = parseInt($(this).attr('id'))

        newProductAdditionalFillings.push(id)
    })

    ProductAdditionalFillings = newProductAdditionalFillings
}

function openAdditionalFillingExistingOptionDialog(event) {
    event.stopPropagation()

    renderAdditionalFillingExistingOptions()

    Dialog.showModal($('#additionalFillingsExistingOptionsDialog'))
}

function renderAdditionalFillingExistingOptions() {
    const options = []
    for (const id in DataProduct.AdditionalFillings) {
        const additionalFilling = DataProduct.AdditionalFillings[id]
        const isSelecetedCurrentOption = ProductAdditionalFillings.includes(additionalFilling.Id)
        const optionItemTemplate = getAdditionalFillingExistTempalte(additionalFilling, isSelecetedCurrentOption)

        options.push(optionItemTemplate)
    }

    $('#additionalFillingsExistingOptionsDialog .additional-filling-list').html(options)
}

function getAdditionalFillingExistTempalte(option, isSelecetedOption) {
    return `
        <div class="funcitons-additional-item-exist">
            <div class="checkbox-item checkbox-item-left">
                <input type="checkbox" value="${option.Id}" ${isSelecetedOption ? 'checked' : ''}/>
            </div>
            <div>${option.Name}</div>
            <div>
                <button class="edit-option-setting-btn" onclick="showEditAdditionalFillingDialog(event, ${option.Id})">
                    <i class="fal fa-edit"></i>
                </button>
            </div>
            <div>
                <button class="remove-option-setting-btn" onclick="removeExistingAdditionalFilling(event, ${option.Id})">
                    <i class="fal fa-trash-alt"></i>
                </button>
            </div>
        </div>
    `
}

function complitedSelectionExistingFillings() {
    ProductAdditionalFillings = []

    $('#additionalFillingsExistingOptionsDialog .additional-filling-list input[type=checkbox]:checked').each(function () {
        const id = parseInt($(this).val())
        ProductAdditionalFillings.push(id)
    })

    renderAdditionalFillingFromProduct()
    Dialog.close('#additionalFillingsExistingOptionsDialog')
}

function removeExistingAdditionalFilling(event, id) {
    event.stopPropagation()

    let callback = function () {
        const loader = new Loader('#additionalFillingsExistingOptionsDialog .dialog-body-function-additional-wrapper')
        loader.start()
        let successFunc = function (result, loader) {
            loader.stop()
            if (result.Success) {
                delete (DataProduct.AdditionalFillings[id])
                renderAdditionalFillingExistingOptions()

                const i = ProductAdditionalFillings.findIndex(p => p == id)
                if (i != -1) {
                    ProductAdditionalFillings.splice(i, 1)
                    
                    renderAdditionalFillingFromProduct()
                }

                if (!Object.keys(DataProduct.AdditionalFillings).length) {
                    Dialog.close('#additionalFillingsExistingOptionsDialog')
                    toggleDisabledExistAdditionalFillingBtn()
                }
            } else {
                showErrorMessage(result.ErrorMessage)
            }
        }
        $.post("/Admin/RemoveAdditionalFilling", { id: id }, successCallBack(successFunc, loader))
    }

    deleteConfirmation(callback)
}