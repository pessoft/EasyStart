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
    const $inputAdditionalInfo = $('#product-additional-info')
    const type = value == ProductAdditionalInfoType.Custom ? 'text' : 'number'

    $inputAdditionalInfo.attr('type', type)
    $btnFunctionAdditionnalInfo.attr('disabled', value == ProductAdditionalInfoType.Custom)
}