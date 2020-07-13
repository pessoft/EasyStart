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

    Dialog.showModal($('#functionAdditionalInfoDialog'))
}

function openAdditionalOptionsDialog(event) {
    event.stopPropagation()

    Dialog.showModal($('#productAdditionalOptionsDialog'))
}

function openCreateAdditionalOptionsDialog(event) {
    event.stopPropagation()

    Dialog.showModal($('#createFunctionAdditionalInfoDialog'))
}

function addRowAdditionalOptionsInDialog(event) {
    event.stopPropagation()

    const dialogId = 'createFunctionAdditionalInfoDialog'
    const $containerProps = $(`#${dialogId} .create-functions-additional`)
    const productAdditionalInfoTypeSelected = parseInt($('#product-additional-info-type option:selected').val())
    const tmpId = generateRandomString() 
    const templateRow = `
        <div class="additional-option-item" id="${tmpId}">
            <div class="additional-option-value">
                <input type="number" min="0" class="default-color default-style-input">
                <span>${ProductAdditionalInfoTypeShortName[productAdditionalInfoTypeSelected]}</span>
            </div>
            
            <div class="additional-option-price">
                <input type="number" min="0" class="default-color default-style-input">
                <span>руб.</span>
            </div>
            
            <div class="additional-option-default-check">
                <input id="${tmpId}-default" type="radio" name="additional-option-default-check">
                <label for="${tmpId}-default">по умлочанию</label>
            </div>
            <button class="remove-options-btn" onclick="removeRowAdditionalOptionsFromDialog(event, this)">
                <i class="fal fa-trash-alt"></i>
            </button>
        </div>
    `

    if ($containerProps.find('.empty-container').length != 0)
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
}

function renderEmptyInfoAdditionalOptionInDialog() {
    const template = `<div class="empty-container">Добавте значения свойства</div>`

    const dialogId = 'createFunctionAdditionalInfoDialog'
    const $containerProps = $(`#${dialogId} .create-functions-additional`)

    $containerProps.html(template);
}