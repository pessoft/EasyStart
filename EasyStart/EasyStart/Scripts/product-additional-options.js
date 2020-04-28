const ProductAdditionalInfoType = {
    Custom: 0,
    Grams: 1,
    Milliliters: 2,
    Liters: 3
}

function onChangeProductAdditaonalInfoType(e) {
    const $e = $(e)
    const value = parseInt($e.val())
    const $btnFunctionAdditionnalInfo = $('#btn-function-product-additional-info')

    $btnFunctionAdditionnalInfo.attr('disabled', value == ProductAdditionalInfoType.Custom)
}