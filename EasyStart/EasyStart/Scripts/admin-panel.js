$(document).ready(function () {
    cheerupServer()

    bindSelectSumo()
    initHistoryOrderDatePicker()
    bindDialogCloseClickBackdor()

    let bindShowModal = function (id, dialogId, additionalFunc, predicate) {
        $(`#${id}`).bind("click", function () {
            let addDialog = $(`#${dialogId}`)

            if (!predicate || predicate()) {
                Dialog.showModal(addDialog)
            }

            if (additionalFunc) {
                additionalFunc()
            }
        })
    }

    $("#setting-phone-number,#setting-phone-number-additional").mask("+7(999)999-99-99")

    $(".main-menu a").not(".logout").bind("click", function () {
        selectMenuItem(this)
    })

    let setOperationAdd = () => CurrentOperation = TypeOperation.Add

    const additionFunForAddNewCategory = () => {
        setOperationAdd()
        toggleRecommendedProductsForCategory()
    }

    const additionFunForAddNewProduct = () => {
        ProductAdditionalOptions = []
        ProductAllowCombinationsAdditionalOptions = []
        ProductAdditionalFillings = []
        setOperationAdd()
        toggleVendorCodeProductInput()
    }

    bindShowModal("add-category", "addCategoryDialog", additionFunForAddNewCategory)
    bindShowModal("add-product", "addProducDialog", additionFunForAddNewProduct, productCondition)
    bindShowModal("add-product-constructor", "addSubCategoryConstructorDialog", initNewCategoryConstructor, productConstructorCondition)
    bindShowModal("add-branch", "addBranchDialog", addNewBranchLoginData)

    $("input[type=file]").not('.not-bind-image').change(function () {
        addPreviewImage(this)
    })

    bindDragula()

    loadMainProductData()
    if ($(".main-menu a.menu-item-active").attr("target-id") == Pages.Order) {
        loadOrders()
    }

    selectToSumoSelectProductType()
    selectToSumoSelectProductAdditionalInfoType()
    selectToSumoSelectCategoryType()
    bindChangePeriodWork()
    bindChangePreorderMinTime()
    bindCustomDialogToggleEvent()

    checkSettings()
})

function toggleRecommendedProductsForCategory(ignoreCategoryId = 0, selectedProducts) {
    const containerQuery = `.recommended-products-wrapper`
    $(`${containerQuery}`).empty()

    if (ProductsForPromotion && Object.keys(ProductsForPromotion).length) {
        const selectId = `recommended-products-list`
        const $select = $(`<select id=${selectId} multiple></select>`)
        const selectOptGroups = []

        for (const categoryId in ProductsForPromotion) {
            if (categoryId == ignoreCategoryId)
                continue

            const categoryName = CategoryDictionary[categoryId]
            const $optGroup = $(`<optgroup label="${categoryName}"></<optgroup>`)
            const options = []

            for (const product of ProductsForPromotion[categoryId]) {
                let selected = ''
                if (selectedProducts && selectedProducts.length) {
                    const isSelected = selectedProducts.findIndex(productId => productId == product.Id) != -1
                    selected = isSelected ? 'selected' : ''
                }

                const option = `<option ${selected} value="${product.Id}">${product.Name}</option>`
                options.push(option)
            }

            $optGroup.html(options)
            selectOptGroups.push($optGroup)
        }

        $select.html(selectOptGroups)

        const header = '<div class="product-type-header">Рекомендуемые товары для категории</div>'
        $(containerQuery).html(header)
        $(containerQuery).append($select)

        const sumoSelectOptions = {
            placeholder: 'Выберите рекомендуемые товары',
            up: true,
            search: true,
            searchText:'Поиск по имени продукта...'
        }
        $(`${containerQuery} #${selectId}`).SumoSelect(sumoSelectOptions)
    } 
}

function toggleVendorCodeProductInput() {
    const query = '#vendor-code-product-wrapper'
    if (IntegerationSystemSetting.Type == IntegrationSystemType.withoutIntegration) {
        $(query).hide()
    } else {
        $(query).show()
    }
}

function checkSettings() {
    if (!IsValidSetting || !IsValidDeliverySetting) {
        $('.main-menu li a').not('.logout').addClass('disabled-menu-item')

        if (!IsValidSetting && !IsValidDeliverySetting) {
            $('[target-id=setting], [target-id=delivery]').removeClass('disabled-menu-item')
            selectMenuItem($('[target-id=setting]'))
        } else if (!IsValidSetting) {
            $('[target-id=setting]').removeClass('disabled-menu-item')
            selectMenuItem($('[target-id=setting]'))
        } else {
            $('[target-id=delivery]').removeClass('disabled-menu-item')
            selectMenuItem($('[target-id=delivery]'))
        }
    } else {
        $('.main-menu li a').removeClass('disabled-menu-item')
    }
}

function getCategoryBySelectCategryId() {
    let selectCategory = null

    if (SelectIdCategoryId > 0 && DataProduct.Categories.length > 0) {
        let selectCategoryes = DataProduct.Categories.filter(p => p.Id == SelectIdCategoryId)
        selectCategory = selectCategoryes.length > 0 ? selectCategoryes[0] : null
    }

    return selectCategory
}

function productCondition() {
    let selectCategory = getCategoryBySelectCategryId()

    if (!selectCategory) {
        showInfoMessage("Выберите категорию")
        return false
    }

    if (selectCategory.CategoryType == CategoryType.Constructor) {
        showInfoMessage("В категорию конструтор нельзя добавить продукт")
        return false
    }

    return true
}

function productConstructorCondition() {
    let selectCategory = getCategoryBySelectCategryId()

    if (!selectCategory) {
        showInfoMessage("Выберите категорию")
        return false
    }

    if (selectCategory.CategoryType == CategoryType.Default) {
        showInfoMessage("В категорию по умолчанию нельзя добавить категорию ингредиентов")
        return false
    }

    return true
}

function bindChangePeriodWork() {
    const $e = $(".period-work-input")
    $e.unbind('change')
    $e.bind('change', function () { onChangeOnlyTime(this) })
    $e.hunterTimePicker()


}

function bindChangePreorderMinTime() {
    const $e = $("#preorder-min-time")
    $e.unbind('change')
    $e.bind('change', function () { onChangeOnlyTime(this, '01:00') })
    $e.hunterTimePicker()
}

function selectToSumoSelectProductType() {
    $("#addProducDialog #product-type").SumoSelect({
        placeholder: 'Присвойте метки',
        okCancelInMulti: true,
        locale: ['ОК', 'Отмена', 'Выбрать все'],
    })
}

function selectToSumoSelectProductAdditionalInfoType() {
    $("#addProducDialog #product-additional-info-type").SumoSelect()
}

function selectToSumoSelectCategoryType() {
    $("#addCategoryDialog #category-type").SumoSelect()
}

function bindDialogCloseClickBackdor() {
    $("dialog").bind('click', function (event) {
        var rect = this.getBoundingClientRect()
        var isInDialog = false

        if (typeof (event.clientY) === typeof (undefined)) {
            isInDialog = true
        }
        else {
            isInDialog = (rect.top <= event.clientY && event.clientY <= rect.top + rect.height
                && rect.left <= event.clientX && event.clientX <= rect.left + rect.width)
        }

        if (!isInDialog) {
            let $dialog = $(this)

            Dialog.clear($dialog)
            Dialog.close($dialog)

            if (ImageProcessingInstance)
                ImageProcessingInstance.destroy()
        }
    })

    $('.custom-dialog').bind('click', function (event) {
        if ($(event.target).hasClass('custom-dialog')) {
            let $dialog = $(this)

            Dialog.clear($dialog)
            Dialog.close($dialog)
        }

    })
}

const CategoryType = {
    Default: 0,
    Constructor: 1
}

function initCategoryTypeName() {
    let categoryTypeName = {}
    categoryTypeName[CategoryType.Default] = 'Стандартная'
    categoryTypeName[CategoryType.Constructor] = 'Конструктор'

    return categoryTypeName

}

const CategoryTypeName = initCategoryTypeName()



const Pages = {
    Order: 'order',
    HistoryOrder: 'history',
    Products: 'products',
    Promotion: 'promotion',
    Branch: 'branch',
    Settong: 'setting',
    Delivery: 'delivery',
    Analytics: 'analytics'
}

var OrderHistoryDatePicker
function initHistoryOrderDatePicker() {
    var prevDate = new Date()
    prevDate.setDate(prevDate.getDate() - 30)

    let options = {
        position: "bottom center",
        range: true,
        multipleDatesSeparator: " - ",
        toggleSelected: false,
        onHide: function (dp, animationCompleted) {
            if (!dp.maxRange && !animationCompleted) {
                dp.selectDate(dp.minRange)
            }

            if (!animationCompleted) {
                loadHistoryOrders()
            }
        }
    }
    let $inputDate = $("#order-history-period")
    $inputDate.datepicker(options)
    OrderHistoryDatePicker = $inputDate.data("datepicker")

    $inputDate.next("i").bind("click", function () {
        OrderHistoryDatePicker.show()
    })

    OrderHistoryDatePicker.selectDate([prevDate, new Date()])
}

var AdditionalBranch = []
var AdditionalHistoryBranch = []
function bindSelectSumo() {
    $("#show-additional-order,#show-additional-history-order").SumoSelect({
        okCancelInMulti: true,
        placeholder: 'Заказы из других городов'
    })

    let updateListenBranchIds = additionalBrunchIds => {
        let currentBrunchId = getCurrentBranchId()
        let listenNewBranchIds = [...additionalBrunchIds, currentBrunchId]

        addListenByBranch(listenNewBranchIds)
    }

    $(`#additional-order-city .btnOk`).bind("click", function () {
        AdditionalBranch = []
        $('#show-additional-order option:selected').each(function (i) {
            AdditionalBranch.push($(this).attr("key"))
        })

        updateListenBranchIds(AdditionalBranch)
        loadOrders(true)
    })

    $(`#additional-history-order-city .btnOk`).bind("click", function () {
        AdditionalHistoryBranch = []
        $('#additional-history-order-city option:selected').each(function (i) {
            AdditionalHistoryBranch.push($(this).attr("key"))
        })

        updateListenBranchIds(AdditionalHistoryBranch)
        loadHistoryOrders()
    })
}

var TypeItem = {
    Categories: 0,
    Products: 1,
    Review: 2
}

function bindDragula() {
    dragula([document.getElementById("category-list")], {
        revertOnSpill: true
    }).on("drop", function () {
        calcOrderNumbers(TypeItem.Categories)
    })

    dragula([document.getElementById("product-list")], {
        revertOnSpill: true
    }).on("drop", function () {
        calcOrderNumbers(TypeItem.Products)
    })

    dragula([$('#functionAdditionalInfoDialog .functions-additional')[0]], {
        revertOnSpill: true
    }).on("drop", function () {
        changeOrderProductAdditionalOption()
    })

    dragula([$('#productAdditionalFillingsDialog .additional-fillng-list')[0]], {
        revertOnSpill: true
    }).on("drop", function () {
        changeOrderProductAdditionalFilling()
    })
}

function calcOrderNumbers(typeItem) {
    let $items = []
    let updaterOrderNumber = []
    let attrName = ""
    let url = ""

    switch (typeItem) {
        case TypeItem.Categories:
            $items = $("#category-list .category-item")
            attrName = "category-id"
            url = "/Admin/UpdateOrderNumberCategory"
            break
        case TypeItem.Products:
            $items = $("#product-list .product-constructor-item")
            attrName = "product-id"
            if ($items.length > 0) {
                url = "/Admin/UpdateOrderNumberConstructorProducts"
            } else {
                $items = $("#product-list .product-item")
                url = "/Admin/UpdateOrderNumberProducts"
            }
            break
    }

    if ($items.length > 0) {
        for (let i = 0; i < $items.length; ++i) {
            let id = $($items[i]).attr(attrName)

            updaterOrderNumber.push({
                Id: id,
                OrderNumber: i + 1,
            })
        }

        $.post(url, { data: updaterOrderNumber }, null)
    }
}

var DataProduct = {
    Categories: [],
    Products: [],
    AdditionalOptions: {},
    AdditionalFillings: {},
}

var TypeOperation = {
    Add: 0,
    Update: 1
}
var CurrentOperation

function addNewBranchLoginData() {
    let login = generateRandomString()
    let password = generateRandomString()

    $("#login-new-branch").val(login)
    $("#password-new-branch").val(password)
}

function logout() {
    let successFunc = (result) => {
        window.location.href = result.URL
    }

    $.post("/Home/Logout", null, successCallBack(successFunc, null))
}

function prevChangedPage(page) {
    switch (page) {
        case Pages.Order:
            resetSearchForOrderNumber(page)
            break
        case Pages.HistoryOrder:
            resetSearchForOrderNumber(page)
            break
    }
}

function postChangedPage(page) {
    switch (page) {
        case Pages.Promotion:
            StockManger.loadStockList()
            CouponManager.loadCoupons()
            NewsManager.loadNewsList()
            CashbackPartners.loadCashbackPartnerSettings()
            break
        case Pages.Analytics: {
            loadAnalyticsReport()
        }
    }
}

function loadAnalyticsReport() {
    const containerId = "analytics-wrapper"
    const currentBrunchId = getCurrentBranchId()

    $(`#${containerId}`).empty()

    new CountOrderReport(containerId, currentBrunchId, URLAnalytics)
    new RevenueReport(containerId, currentBrunchId, URLAnalytics)
    new Top5Categories(containerId, currentBrunchId, URLAnalytics)
    new Top5Products(containerId, currentBrunchId, URLAnalytics)
    new DeliveryMethod(containerId, currentBrunchId, URLAnalytics)
    new NewUsersReport(containerId, currentBrunchId, URLAnalytics)
    new ActiveUsersReport(containerId, currentBrunchId, URLAnalytics)
    new GeneralUserQuantityrReport(containerId, currentBrunchId, URLAnalytics)
}

function resetSearchForOrderNumber(containerId) {
    clearSearchInput(containerId)
    searchByOrderNumber(containerId, false)
}

function clearSearchInput(containerId) {
    $(`#${containerId} .search-input input`).val("")
}

function selectMenuItem(e) {
    let $e = $(e)
    let targetId = $e.attr("target-id")

    if ($e.hasClass("menu-item-active")) {
        return
    }

    prevChangedPage(targetId)

    $(".main-menu a").removeClass("menu-item-active")
    $e.addClass("menu-item-active")
    $(".section").addClass("hide")
    $(`#${targetId}`).removeClass("hide")

    postChangedPage(targetId)
}

function cancelDialog(e) {
    let dialog = $(e)

    Dialog.clear(dialog)
    Dialog.close(dialog)
}

function operationCategory() {
    switch (CurrentOperation) {
        case TypeOperation.Add:
            addCategory()
            break
        case TypeOperation.Update:
            updateCategory()
            break
    }
}

function operationProduct() {
    if (SelectIdCategoryId <= 0) {
        showInfoMessage("Выберите категорию")
    }

    switch (CurrentOperation) {
        case TypeOperation.Add:
            addProduct()
            break
        case TypeOperation.Update:
            updateProduct()
            break
    }
}

var SelectIdCategoryId = -1

function updateCategory() {
    let loader = new Loader($("#addCategoryDialog form"))
    loader.start()

    let category = {
        Id: $("#category-id").val(),
        Name: $("#name-category").val(),
        Image: $("#addCategoryDialog img").attr("src"),
        NumberAppliances: $("#addCategoryDialog #number-appliances").is(':checked'),
        RecommendedProducts: getRecommendedProductsFromDialog($("#recommended-products-list option:selected"))
    }

    let successFunc = function (result, loader) {
        loader.stop()
        if (result.Success) {
            const index = DataProduct.Categories.findIndex(p => p.Id == category.Id)

            if (index != -1)
                DataProduct.Categories[index] = result.Data
            let categoryItem = $(`[category-id=${category.Id}]`)
            categoryItem.find(".category-item-image img").attr("src", category.Image)
            categoryItem.find(".category-item-name").html(category.Name)
            categoryItem.find(".number-appliances-data").val(category.NumberAppliances ? 'true' : '')

            cancelDialog("#addCategoryDialog")
        } else {
            showErrorMessage(result.ErrorMessage)
        }
    }

    $.post("/Admin/UpdateCategory", category, successCallBack(successFunc, loader)).catch(function () {
        errorFunc()
    })
}

function getRecommendedProductsFromDialog($items) {
    const itemsValue = []

    $items.each(function () {
        itemsValue.push(parseInt($(this).attr('value')))
    })

    return itemsValue
}

function addCategory() {
    let loader = new Loader($("#addCategoryDialog form"))
    loader.start()

    let category = {
        Name: $("#name-category").val(),
        CategoryType: parseInt($("#addCategoryDialog #category-type").val()),
        Image: $("#addCategoryDialog img").attr("src"),
        NumberAppliances: $("#addCategoryDialog #number-appliances").is(':checked'),
        RecommendedProducts: getRecommendedProductsFromDialog($("#recommended-products-list option:selected"))
    }

    let successFunc = function (result, loader) {
        loader.stop()
        if (result.Success) {
            DataProduct.Categories.push(result.Data)
            addCategoryInCategoryDictionary(result.Data)
            $(".category .empty-list").remove()
            addCategoryToList(result.Data)
            cancelDialog("#addCategoryDialog")
        } else {
            showErrorMessage(result.ErrorMessage)
        }
    }

    $.post("/Admin/AddCategory", category, successCallBack(successFunc, loader)).catch(function () {
        errorFunc()
    })
}

function addCategoryToList(category) {
    let templateCategoryItem = `
    <div class="category-item" onclick="selectCategory(this)" category-id="${category.Id}" category-type="${category.CategoryType}">
        <div class="category-item-image">
            <img src="${category.Image}" />
        </div>
        <div class="category-item-name">
            ${category.Name}
        </div>
        <input type="hidden" class="number-appliances-data" value="${(category.NumberAppliances ? 'true' : '')}">
        <div class="category-item-action">
            <i onclick="editCategory(this, event)" class="fal fa-edit"></i>
            <i class="fal fa-eye item-show ${(category.Visible ? '' : 'hide')}" onclick="toggleShowItem(this, ${TypeItem.Categories}, event)"></i>
            <i class="fal fa-eye-slash item-hide ${(category.Visible ? 'hide' : '')}" onclick="toggleShowItem(this, ${TypeItem.Categories}, event)"></i>
            <i onclick="removeCategory(this, event)" class="fal fa-trash-alt"></i>
        </div>
    </div >`

    $(".category-list").append(templateCategoryItem)
}

function editCategory(e, event) {
    event.stopPropagation()

    CurrentOperation = TypeOperation.Update

    let dialog = $("#addCategoryDialog")
    let parent = $($(e).parents(".category-item"))

    const categoryId = parent.attr("category-id")
    const cotegoryFromData = DataProduct.Categories.find(p => p.Id == categoryId)
    let category = {
        Id: categoryId,
        CategoryType: parseInt(parent.attr("category-type")),
        Name: parent.find(".category-item-name").html().trim(),
        Image: parent.find("img").attr("src"),
        NumberAppliances: !!parent.find(".number-appliances-data").val(),
        RecommendedProducts: cotegoryFromData.RecommendedProducts
    }

    

    dialog.find("#category-id").val(category.Id)
    dialog.find("#name-category").val(category.Name)
    dialog.find("img").attr("src", category.Image)

    dialog.find("#number-appliances").prop('checked', category.NumberAppliances)

    if (category.Image.indexOf("default") == -1) {
        dialog.find("img").removeClass("hide")
        dialog.find(".dialog-image-upload").addClass("hide")
    }

    Dialog.showModal(dialog)
    toggleRecommendedProductsForCategory(category.Id, category.RecommendedProducts)

    const $select = $("#addCategoryDialog #category-type")

    if ($select[0] && $select[0].sumo) {
        if (category.Id > 0) {
            $select[0].sumo.unSelectAll()
            $select[0].sumo.selectItem(category.CategoryType.toString())
            $select[0].sumo.disable()
        } else {
            $select[0].sumo.enable()
            $select[0].sumo.selectItem(CategoryType.Default.toString())

        }
    }
}

function addCategoryInCategoryDictionary(category) {
    CategoryDictionary[category.Id] = category.Name
}

function removeCategoryFromCategoryDictionary(categoryId) {
    delete CategoryDictionary[categoryId]
}

function removeProductsByCategoryIdFromProductsPromotion(categoryId) {
    delete ProductsForPromotion[categoryId]
}

function removeProductsByIdFromProductsPromotion(productId) {
    let products = ProductsForPromotion[SelectIdCategoryId]

    if (products && products.length > 0) {
        const index = products.findIndex(p => p.Id == productId)

        if (index >= 0) {
            products.splice(index, 1)

            if (products.length == 0)
                delete ProductsForPromotion[SelectIdCategoryId]
        }
    }

}

function addProductsByCategoryIdInProductsPromotion(product) {
    if (!ProductsForPromotion[SelectIdCategoryId]) {
        ProductsForPromotion[SelectIdCategoryId] = [product]
    } else {
        ProductsForPromotion[SelectIdCategoryId].push(product)
    }
}

function removeCategory(e, event) {
    event.stopPropagation()

    let callback = function () {
        let parent = $($(e).parents(".category-item"))
        let id = parent.attr("category-id")
        let loader = new Loader(parent)
        let successFunc = function (result, loader) {
            loader.stop()
            if (result.Success) {
                removeProductsByCategoryIdFromProductsPromotion(id)
                removeCategoryFromCategoryDictionary(id)

                if (SelectIdCategoryId == id) {
                    SelectIdCategoryId = -1

                    clearProductList()
                    setEmptyProductInfo()
                }

                $(`[category-id=${id}]`).fadeOut(500, function () {
                    $(this).remove()

                    if ($(".category-list").children().length == 0) {
                        setEmptyCategoryInfo()
                    }
                })

            } else {
                showErrorMessage(result.ErrorMessage)
            }
        }
        loader.start()

        $.post("/Admin/RemoveCategory", { id: id }, successCallBack(successFunc, loader))
    }


    deleteConfirmation(callback)
}

function selectCategory(e) {
    $(".category-list .select-category").removeClass("select-category")
    $(e).addClass("select-category")
    let categoryId = $(e).attr("category-id")
    let categoryType = $(e).attr("category-type")

    if (categoryId == SelectIdCategoryId) {
        return
    }

    SelectIdCategoryId = categoryId
    changeButtonAddProduct()

    if (categoryType == CategoryType.Default)
        loadProductList(SelectIdCategoryId)
    else
        loadProductConstructorList(SelectIdCategoryId)
}


var CustomToggleElement = {
    show: function (queryItem) {
        $(queryItem).removeClass('hide')
    },
    hide: function (queryItem) {
        $(queryItem).addClass('hide')
    },
    toggle: function (queryItem) {
        let $item = $(queryItem)

        if ($item.hasClass('hide'))
            this.show(queryItem)
        else
            this.hide(queryItem)
    }
}

function changeButtonAddProduct() {
    let selectCategory = getCategoryBySelectCategryId()

    if (selectCategory) {

        switch (selectCategory.CategoryType) {
            case CategoryType.Default:
                CustomToggleElement.show('#add-product')
                CustomToggleElement.hide('#add-product-constructor')
                break
            case CategoryType.Constructor:
                CustomToggleElement.show('#add-product-constructor')
                CustomToggleElement.hide('#add-product')
                break
        }

    }
}

function sortByOrderNumber(data) {
    var newData = []

    for (var item of data) {
        newData[item.OrderNumber - 1] = item
    }

    return newData
}

function loadMainProductData() {
    SelectIdCategoryId = -1
    clearCategoryList()

    DataProduct.Products = []
    let container = $(".category-list")
    let loader = new Loader($(".category"))
    let successFunc = function (result, loader) {
        loader.stop()
        if (result.Success) {
            if (!result.Data
                || !result.Data.Categories
                || result.Data.Categories.length == 0) {
                setEmptyCategoryInfo()
            } else {
                DataProduct.Categories = sortByOrderNumber(result.Data.Categories)
                DataProduct.AdditionalOptions = convertListToDictionary(result.Data.AdditionalOptions)
                DataProduct.AdditionalFillings = convertListToDictionary(result.Data.AdditionalFillings)

                for (let category of DataProduct.Categories) {
                    addCategoryToList(category)
                }
            }

        } else {
            showErrorMessage(result.ErrorMessage)
            setEmptyCategoryInfo()
        }
    }
    loader.start()

    $.post("/Admin/LoadMainProductData", null, successCallBack(successFunc, loader))
}

function convertListToDictionary(items) {
    let dict = {}

    if (items && items.length != 0) {
        for (const item of items)
            dict[item.Id] = item
    }

    return dict
}

function loadProducts() {
    clearCategoryList()
    clearProductList()
    setEmptyProductInfo()

    loadMainProductData()
}

var ProductAdditionalOptions = []
var ProductAllowCombinationsAdditionalOptions = []
var ProductAdditionalFillings = []
function addProduct() {
    let loader = new Loader($("#addProducDialog  form"))
    loader.start()

    let product = {
        CategoryId: SelectIdCategoryId,
        Name: $("#name-product").val(),
        AdditionInfo: $("#product-additional-info").val(),
        Price: $("#product-price").val(),
        Description: $("#description-product").val(),
        Image: $("#addProducDialog img").attr("src"),
        ProductType: getProductType($("#product-type option:selected")),
        ProductAdditionalInfoType: $("#product-additional-info-type").val(),
        ProductAdditionalOptionIds: ProductAdditionalOptions,
        AllowCombinationsJSON: JSON.stringify(ProductAllowCombinationsAdditionalOptions),
        ProductAdditionalFillingIds: ProductAdditionalFillings,
        VendorCode: $('#vendor-code-product').val()
    }
    let successFunc = function (result, loader) {
        loader.stop()
        if (result.Success) {
            $(".product .empty-list").remove()
            DataProduct.Products.push(result.Data)
            addProductsByCategoryIdInProductsPromotion(result.Data)
            addProductToList(result.Data)
            cancelDialog("#addProducDialog")
        } else {
            showErrorMessage(result.ErrorMessage)
        }
    }

    $.post("/Admin/AddProduct", product, successCallBack(successFunc, loader)).catch(function () {
        errorFunc()
    })
}

function updateProduct() {
    let loader = new Loader($("#addProducDialog form"))
    loader.start()

    let updateProduct = {
        Id: $("#product-id").val(),
        CategoryId: SelectIdCategoryId,
        Name: $("#name-product").val(),
        AdditionInfo: $("#product-additional-info").val(),
        Price: $("#product-price").val(),
        Description: $("#description-product").val(),
        Image: $("#addProducDialog img").attr("src"),
        ProductType: getProductType($("#product-type option:selected")),
        ProductAdditionalInfoType: $("#product-additional-info-type").val(),
        ProductAdditionalOptionIds: ProductAdditionalOptions,
        AllowCombinationsJSON: JSON.stringify(ProductAllowCombinationsAdditionalOptions),
        ProductAdditionalFillingIds: ProductAdditionalFillings,
        VendorCode: $('#vendor-code-product').val()
    }

    let successFunc = function (result, loader) {
        loader.stop()
        if (result.Success) {
            const product = result.Data
            const index = DataProduct.Products.findIndex(p => p.Id == product.Id)

            if (index != -1)
                DataProduct.Products[index] = result.Data

            let productItem = $(`[product-id=${product.Id}]`)
            productItem.find(".product-item-image img").attr("src", product.Image)
            productItem.find(".product-item-name").html(product.Name)

            const additionalInfo = getProductAdditionalInfoTypeStr(product)
            const price = getProductGeneralPrice(product)
            productItem.find(".product-item-additional-info").html(additionalInfo)
            productItem.find(".product-item-price span").html(price)
            productItem.find(".product-item-description").html(product.Description)
            productItem.find(".product-type-item").html(product.ProductType)
            productItem.find(".product-additional-info-value").html(product.ProductAdditionalInfoType)

            cancelDialog("#addProducDialog")
        } else {
            showErrorMessage(result.ErrorMessage)
        }
    }

    $.post("/Admin/UpdateProduct", updateProduct, successCallBack(successFunc, loader)).catch(function () {
        errorFunc()
    })
}

function toggleShowItem(e, typeItem, event) {
    if (event) {
        event.stopPropagation()
    }

    let url = ""
    let $e = $(e)
    let $parent
    let id
    let visible
    switch (typeItem) {
        case TypeItem.Categories:
            $parent = $e.parents(".category-item")
            id = $parent.attr("category-id")
            url = "/Admin/UpdateVisibleCategory"
            break
        case TypeItem.Products:
            $parent = $e.parents(".product-item")
            id = $parent.attr("product-id")
            url = "/Admin/UpdateVisibleProduct"
            break
        case TypeItem.Review:
            $parent = $e.parents(".review-item")
            id = $parent.attr("review-id")
            url = "/Admin/UpdateVisibleReview"
            break
    }

    $e.addClass("hide")

    if ($e.hasClass("item-show")) {
        visible = false
        $parent.find(".item-hide").removeClass("hide")
    } else {
        visible = true
        $parent.find(".item-show").removeClass("hide")
    }

    let updaterVisible = {
        Id: id,
        Visible: visible

    }
    $.post(url, { data: updaterVisible }, null)

}

function addProductConstructorToList(product) {
    let templateCategoryItem = `
    <div class="product-item product-constructor-item" category-id="${product.CategoryId}" product-id="${product.Id}">
        <div class="product-item-header">
            <div class="product-item-image">
                <i class="fal fa-cogs"></i>
            </div>
            <div class="product-item-name">
                ${product.Name}
            </div>
            <div class="product-item-additional-info">
                ${product.Ingredients.length} ${num2str(product.Ingredients.length, ['ингредиент', 'ингредиента', 'ингредиентов'])}
            </div>
            <div class="product-item-action">
                <i onclick="editCategoryConstructor(${product.Id}, event)" class="fal fa-edit"></i>
                <i onclick="removeCategoryConstructor(${product.Id}, event)" class="fal fa-trash-alt"></i>
            </div>
        </div>
    </div >`

    let $template = $(templateCategoryItem)
    $(".product-list").append($template)
}

function updateProductConstructorToList(product) {
    let $productContructorItem = $(`.product-constructor-item[product-id="${product.Id}"]`)

    if ($productContructorItem.length > 0) {
        $productContructorItem.find('.product-item-name').html(product.Name)

        const additionalInfo = `${product.Ingredients.length} ${num2str(product.Ingredients.length, ['ингредиент', 'ингредиента', 'ингредиентов'])}`
        $productContructorItem.find('.product-item-additional-info').html(additionalInfo)
    }
}

function getProductAdditionalInfoTypeStr(product) {
    let additionInfo

    if (product.ProductAdditionalInfoType != ProductAdditionalInfoType.Custom) {
        additionInfo = parseFloat(product.AdditionInfo)

        if (product.ProductAdditionalOptionIds.length) {
            for (const id of product.ProductAdditionalOptionIds) {
                const option = DataProduct.AdditionalOptions[id]
                const infoItem = option.Items.find(p => p.IsDefault)

                additionInfo += infoItem.AdditionalInfo
            }

            additionInfo = xFormatPrice(additionInfo)
        }

    } else
        additionInfo = product.AdditionInfo.trim()

    const additionalInfoStr = `${additionInfo} ${ProductAdditionalInfoTypeShortName[product.ProductAdditionalInfoType]}`

    return additionalInfoStr
}

function getProductGeneralPrice(product) {
    let price = product.Price

    if (product.ProductAdditionalInfoType != ProductAdditionalInfoType.Custom) {
        additionInfo = parseFloat(product.AdditionInfo)

        if (product.ProductAdditionalOptionIds.length) {
            for (const id of product.ProductAdditionalOptionIds) {
                const option = DataProduct.AdditionalOptions[id]
                const infoItem = option.Items.find(p => p.IsDefault)

                price += infoItem.Price
            }
        }

    }

    return xFormatPrice(price)
}

function addProductToList(product) {
    let templateCategoryItem = `
    <div class="product-item" category-id="${product.CategoryId}" product-id="${product.Id}">
        <div class="product-item-header">
            <div class="product-item-image">
                <img src="${product.Image}" />
            </div>
            <div class="product-item-name">
                ${product.Name}
            </div>
            <div class="product-item-raty">
            </div>
            <div class="product-item-additional-info">
                ${getProductAdditionalInfoTypeStr(product)}
            </div>
            <div class="product-item-price">
                <span>${getProductGeneralPrice(product)}</span>
                <i class="fal fa-ruble-sign"></i>
            </div>
            <div class="product-item-action">
                <i onclick="editProduct(this, event)" class="fal fa-edit"></i>
                <i class="fal  fa-comment-dots" onclick="openProductUserCallback(this, event)"></i>
                <i class="fal fa-eye item-show ${(product.Visible ? '' : 'hide')}" onclick="toggleShowItem(this, ${TypeItem.Products}, event)"></i>
                <i class="fal fa-eye-slash item-hide ${(product.Visible ? 'hide' : '')}" onclick="toggleShowItem(this, ${TypeItem.Products}, event)"></i>
                <i onclick="removeProduct(this, event)" class="fal fa-trash-alt"></i>
            </div>
        </div>
        <div class="product-item-description hide">
                ${product.Description}
            </div>
        <div class="product-type-item hide">
            ${product.ProductType}
        </div>
        <div class="product-additional-info-value hide">
            ${product.ProductAdditionalInfoType}
        </div>
    </div >`

    let $template = $(templateCategoryItem)

    $template.find(".product-item-raty").raty({
        score: product.Rating,
        showHalf: true,
        path: "../images/rating",
        targetKeep: true,
        precision: true,
        readOnly: true
    })

    $(".product-list").append($template)
}

function clearProductList() {
    $(".product-list").empty()
}

function clearCategoryList() {
    $(".category-list").empty()
}

function editProduct(e, event) {
    event.stopPropagation()

    CurrentOperation = TypeOperation.Update

    let dialog = $("#addProducDialog")
    let parent = $($(e).parents(".product-item"))
    const productId = parent.attr("product-id")

    let product = DataProduct.Products.find(p => p.Id == productId)
    ProductAdditionalOptions = product.ProductAdditionalOptionIds
    ProductAllowCombinationsAdditionalOptions = product.AllowCombinations ? product.AllowCombinations : []
    ProductAdditionalFillings = product.ProductAdditionalFillingIds

    dialog.find("#product-id").val(product.Id)
    dialog.find("#name-product").val(product.Name)
    dialog.find("#product-additional-info").val(product.AdditionInfo)

    let productAdditionalInfoTypeSumo = dialog.find("#product-additional-info-type")[0].sumo
    productAdditionalInfoTypeSumo.unSelectAll()
    productAdditionalInfoTypeSumo.selectItem(product.ProductAdditionalInfoType)

    dialog.find('#btn-function-product-additional-info').attr('disabled', product.ProductAdditionalInfoType == ProductAdditionalInfoType.Custom)
    dialog.find('#btn-product-options').attr('disabled', product.ProductAdditionalInfoType == ProductAdditionalInfoType.Custom)
    dialog.find("#product-price").val(product.Price)
    dialog.find("#vendor-code-product").val(product.VendorCode)
    dialog.find("#description-product").val(product.Description)
    dialog.find("img").attr("src", product.Image)

    if (product.Image.indexOf("default") == -1) {
        dialog.find("img").removeClass("hide")
        dialog.find(".dialog-image-upload").addClass("hide")
    }

    toggleVendorCodeProductInput()
    Dialog.showModal(dialog)

    setProductType(product.ProductType)//работает только если show, поэтому вызывается после покада диалогового окна
}

function deleteConfirmation(callback) {
    let $dialog = $("#deleteConfirmationDialog")
    let clickFunc = function () {
        if (callback) {
            callback()
        }

        cancelDialog($dialog)
    }

    $dialog.find(".btn-submit").unbind("click")
    $dialog.find(".btn-submit").bind("click", clickFunc)

    Dialog.showModal($dialog)
}

function removeProduct(e, event) {
    event.stopPropagation()

    let callback = function () {
        let parent = $($(e).parents(".product-item"))
        let id = parent.attr("product-id")
        let loader = new Loader(parent)
        let successFunc = function (result, loader) {
            loader.stop()
            if (result.Success) {
                removeProductsByIdFromProductsPromotion(id)
                $(`[product-id=${id}]`).fadeOut(500, function () {
                    $(this).remove()

                    if ($(".product-list").children().length == 0) {
                        setEmptyProductInfo()
                    }
                })

            } else {
                showErrorMessage(result.ErrorMessage)
            }
        }
        loader.start()

        $.post("/Admin/RemoveProduct", { id: id }, successCallBack(successFunc, loader))
    }

    deleteConfirmation(callback)
}


function getProductById(id) {
    let serchProduct = null

    for (let product of DataProduct.Products) {
        if (product.Id == id) {
            serchProduct = product
            break
        }
    }

    return serchProduct
}

function loadProductConstructorList(idCategory) {
    clearProductList()
    let loader = new Loader($(".product"))
    let successFunc = function (result, loader) {
        loader.stop()
        if (result.Success) {
            if (!result.Data || result.Data.length == 0) {
                setEmptyProductInfo()
            } else {
                DataProduct.Products = result.Data
                for (let product of DataProduct.Products) {
                    addProductConstructorToList(product)
                }
            }
        } else {
            showErrorMessage(result.ErrorMessage)
            setEmptyProductInfo()
        }
    }
    loader.start()

    $.post("/Admin/LoadProductConstructorList", { idCategory: idCategory }, successCallBack(successFunc, loader))
}

function loadProductList(idCategory) {
    clearProductList()
    let loader = new Loader($(".product"))
    let successFunc = function (result, loader) {
        loader.stop()
        if (result.Success) {
            if (!result.Data || result.Data.length == 0) {
                setEmptyProductInfo()
            } else {
                DataProduct.Products = sortByOrderNumber(result.Data)
                for (let product of DataProduct.Products) {
                    addProductToList(product)
                }


            }
        } else {
            showErrorMessage(result.ErrorMessage)
            setEmptyProductInfo()
        }
    }
    loader.start()

    $.post("/Admin/LoadProductList", { idCategory: idCategory }, successCallBack(successFunc, loader))
}

function addPreviewImage(input) {
    if (input.files && input.files[0]) {
        let dialog = $(input).parents("dialog")
        dialog = dialog.length > 0 ? dialog : $(input).parents(".custom-dialog")

        if (dialog.length == 0)
            return

        const aspectRatioName = $(input).attr('aspect-ratio')
        let action = src => setDialogPreviewImage(dialog, src)
        ImageProcessingInstance = new ImageProcessing(input, action, AspectRatioType[aspectRatioName])
    }
}

function setDialogPreviewImage(dialog, src) {
    let img = dialog.find("img")

    dialog.find(".dialog-image-upload").addClass("hide")
    img.attr("src", src)
    img.removeClass("hide")
}

function openDialogFile(id) {
    $(`#${id}`).click()
}

function saveSetting() {
    let warnMsg = {
        City: "Выберите город из списка",
        Street: "Укажите имя улицы",
        HomeNumber: "Укажите номер дома",
        PhoneNumber: "Укажите номер телефона"
    }

    let cityId = $("#setting-city-list option[value='" + $('#setting-city').val() + "']").attr('city-id')
    let street = $("#setting-street").val()
    let homeNumber = $("#setting-home").val()
    let phoneNumber = $("#setting-phone-number").val()

    if (!cityId) {
        showWarningMessage(warnMsg.City)
        return
    }
    if (!street) {
        showWarningMessage(warnMsg.Street)
        return
    }
    if (!homeNumber) {
        showWarningMessage(warnMsg.HomeNumber)
        return
    }
    if (!phoneNumber) {
        showWarningMessage(warnMsg.PhoneNumber)
        return
    }

    let setting = {
        Id: $("#setting").attr("setting-id"),
        CityId: cityId,
        Street: street,
        PhoneNumber: phoneNumber,
        PhoneNumberAdditional: $("#setting-phone-number-additional").val(),
        Email: $("#setting-email").val(),
        HomeNumber: homeNumber,
        Vkontakte: $("#setting-vk").val(),
        Instagram: $("#setting-instagram").val(),
        Facebook: $("#setting-facebook").val(),

        PriceDelivery: $("#price-delivery").val(),
        FreePriceDelivery: $("#free-delivery").val(),
        TimeOpen: parseFloat($("#time-open").val()).toFixed(2).toString(),
        TimeClose: parseFloat($("#time-close").val()).toFixed(2),
    }
    let loader = new Loader($("#setting"))
    let successFunc = function (result, loader) {
        loader.stop()
        if (!result.Success) {
            showErrorMessage(result.ErrorMessage)
        } else {
            IsValidSetting = true

            checkSettings()
        }
    }

    loader.start()

    $.post("/Admin/SaveSetting", setting, successCallBack(successFunc, loader))
}

var DayWeekly = {
    Monday: 1,
    Tuesday: 2,
    Wednesday: 3,
    Thursday: 4,
    Friday: 5,
    Saturday: 6,
    Sunday: 7
}

function getTimeDeliveryJSON() {
    let timeDays = {}
    for (let day in DayWeekly) {
        let timeDay = $(`[day-id=${day}]`)
        let checked = timeDay.find("input[type=checkbox]").is(":checked")
        let start = timeDay.find("[name=start]").val()
        let end = timeDay.find("[name=end]").val()

        timeDays[DayWeekly[day]] = checked ? [start, end] : null
    }

    return JSON.stringify(timeDays)
}

function saveDeliverySetting() {
    let warnMgs = {
        AreaDelivery: "Настройте районы доставки",
        WorkTime: "Укажите режим работы",
        PreorderTime: "Натройте раздел предзаказ",
        DeliveryType: "Добавте способы выдачи заказа",
    }

    let maxPreorderPeriod = $('#preorder-max-date').val()
    let minTimeProcessingOrder = $('#preorder-min-time').val()

    if (!maxPreorderPeriod || !minTimeProcessingOrder) {
        showWarningMessage(warnMgs.PreorderTime)
        return
    }

    if (!AreaDelivery || AreaDelivery.length == 0) {
        showWarningMessage(warnMgs.AreaDelivery)
        return
    }

    if (!$('#delivery .table-time input[type=checkbox]').is(':checked')) {
        showWarningMessage(warnMgs.WorkTime)
        return
    }

    const isDelivery = $("#setting-delivery-type").is(":checked")
    const isTakeYourSelf = $("#setting-takeyourself-type").is(":checked")

    if (!isDelivery && !isTakeYourSelf) {
        showWarningMessage(warnMgs.DeliveryType)
        return
    }

    let setting = {
        IsSoundNotify: $("#sound-nodify").is(":checked"),
        NotificationEmail: $("#notify-email").val(),
        ZoneId: $("#delivery-time-zone").val(),
        PayCard: $("#payment-card").is(":checked"),
        PayCash: $("#payment-cash").is(":checked"),
        IsDelivery: isDelivery,
        IsTakeYourSelf: isTakeYourSelf,
        TimeDeliveryJSON: getTimeDeliveryJSON(),
        AreaDeliveries: AreaDelivery,
        MaxPreorderPeriod: maxPreorderPeriod,
        MinTimeProcessingOrder: minTimeProcessingOrder,
        PayOnline: OnlinePayData.isPayOnline,
        MerchantId: OnlinePayData.merchantId,
        PaymentKey: OnlinePayData.paymentKey,
        CreditKey: OnlinePayData.creditKey,
        IsAcceptedOnlinePayCondition: OnlinePayData.isAcceptedOnlinePayCondition
    }
    let loader = new Loader($("#delivery"))
    let successFunc = function (result, loader) {
        loader.stop()
        if (!result.Success) {
            showErrorMessage(result.ErrorMessage)
        } else {
            IsValidDeliverySetting = true

            checkSettings()
        }
    }

    loader.start()

    $.post("/Admin/SaveDeliverySetting", setting, successCallBack(successFunc, loader))
}

function addNewBranchToAdditionalOrder(brachId, cityName) {
    let option = `<option key="${brachId}">${cityName}</option>`
    let $additionaOrder = $("#show-additional-order")
    let $additionaOrderHistory = $("#show-additional-history-order")

    $additionaOrder.append(option)
    $additionaOrder[0].sumo.reload()
    $additionaOrderHistory.append(option)
    $additionaOrderHistory[0].sumo.reload()
}

function removeBranchFromAdditionalOrder(brachId) {
    let $additionaOrder = $("#show-additional-order")
    let $additionaOrderHistory = $("#show-additional-history-order")

    $additionaOrder.find(`option[key=${brachId}]`).remove()
    $additionaOrder[0].sumo.reload()
    $additionaOrderHistory.find(`option[key=${brachId}]`).remove()
    $additionaOrderHistory[0].sumo.reload()
}

function addBranch() {
    let newBranch = {
        Login: $("#login-new-branch").val(),
        Password: $("#password-new-branch").val(),
        CityId: $("#branch-city-list option[value='" + $('#branch-city').val() + "']").attr('city-id'),
    }
    let loader = new Loader($("#addBranchDialog form"))
    let successFunc = function (result, loader) {
        loader.stop()
        if (result.Success) {
            addBranchToList(result.Data)
            cancelDialog("#addBranchDialog")
        } else {
            showErrorMessage(result.ErrorMessage)
        }
    }
    loader.start()

    $.post("/Admin/AddBranch", newBranch, successCallBack(successFunc, loader))
}

function addBranchToList(branchView) {
    let templateBranchItem = `
    <div class="branch-item" branch-id="${branchView.Id}">
        <div class="branch-item-info">
            <div class="branch-adress">${branchView.Addres}</div>
            <div class="branch-phone-number">${branchView.PhoneNumber}</div>
            <div class="branch-login">${branchView.Login}</div>
            <div class="branch-password">${branchView.Password}</div>
        </div>
        <div onclick="removeBranch(this, ${branchView.Id})" class="branch-item-action ${branchView.Login.indexOf('******') != -1 ? 'disbled' : ''}">
            <i class="fal fa-trash-alt"></i>
        </div>
    </div >`

    $(".branch-list").append(templateBranchItem)
}

function loadBranchList() {
    let container = $(".branch-list")
    container.empty()
    let loader = new Loader(container)
    let successFunc = function (result, loader) {
        loader.stop()
        if (result.Success) {
            for (let branchView of result.Data) {
                addBranchToList(branchView)
            }

        } else {
            showErrorMessage(result.ErrorMessage)
        }
    }
    loader.start()

    $.post("/Admin/LoadBranchList", null, successCallBack(successFunc, loader))
}

function removeBranch(e, id) {
    if ($(e).hasClass("disbled")) {
        return
    }

    const callback = () => {
        let parent = $($(e).parents(".branch-item"))
        let loader = new Loader(parent)
        let successFunc = function (result, loader) {
            loader.stop()
            if (result.Success) {
                $(`[branch-id=${id}]`).fadeOut(500, function () {
                    $(this).remove()
                })
            } else {
                showErrorMessage(result.ErrorMessage)
            }
        }
        loader.start()

        $.post("/Admin/RemoveBranch", { id: id }, successCallBack(successFunc, loader))
    }

    deleteConfirmation(callback)
}

function setEmptyProductInfo() {
    let template = `
        <div class="empty-list">
            <i class="fal fa-smile-plus"></i>
            <span>Выберите категорию и добавте продукт</span>
        </div>
    `

    $(".product-list").html(template)
}

function setEmptyCategoryInfo() {
    let template = `
        <div class="empty-list">
            <i class="fal fa-smile-plus"></i>
            <span>Пока нет ни одной категории</span>
        </div>
    `

    $(".category-list").html(template)
}

function setEmptyReview() {
    let template = `
        <div class="empty-list">
            <i class="fal fa-comment-edit"></i>
            <span>Пока нет ни одного отзыва</span>
        </div>
    `

    $("#productUserCallbackDialog .product-user-callback-review-list").append(template)
}

function renderReviewItem(data) {
    let $template = $($("#review-item-template").html())
    let userName = `Клиент: ${data.PhoneNumber}`


    $template.attr("review-id", data.Id)
    $template.find(".review-item-header span").html(userName)
    $template.find(".review-item-text").html(data.ReviewText)

    if (data.Visible) {
        $template.find(".item-show").removeClass("hide")
        $template.find(".item-hide").addClass("hide")
    } else {
        $template.find(".item-show").addClass("hide")
        $template.find(".item-hide").removeClass("hide")
    }

    return $template
}

function cleanProductUserCallbackDialog() {
    let $dialog = $("#productUserCallbackDialog")

    $dialog.find("img").not('.no-clean').attr("src", "")
    $dialog.find(".product-name-user-callback").html("")
    $dialog.find(".product-user-callback-review-list").html("")
    $dialog.find(".product-raty-text-user-callback").html("")
}

function num2str(n, text_forms) {
    n = Math.abs(n) % 100
    let n1 = n % 10

    if (n > 10 && n < 20) { return text_forms[2] }
    if (n1 > 1 && n1 < 5) { return text_forms[1] }
    if (n1 == 1) { return text_forms[0] }

    return text_forms[2]
}

function openProductUserCallback(e, event) {
    if (event) {
        event.stopPropagation()
    }

    cleanProductUserCallbackDialog()
    Reviews = []

    let $parent = $(e).parents(".product-item")
    let loader = new Loader($parent)

    loader.start()

    let productId = $parent.attr("product-id")
    let callback = () => {
        let templateReviews = []
        for (let review of Reviews) {
            templateReviews.push(renderReviewItem(review))
        }

        let product = getProductById(productId)
        var votesCountStr = num2str(product.VotesCount, ["голос", "голоса", "голосов"])
        let reviewText = `Оценка: ${product.Rating.toFixed(1)} - ${product.VotesCount} ${votesCountStr}`

        let $dialog = $("#productUserCallbackDialog")

        let $img = $dialog.find("img")
        $img.attr("src", product.Image)
        $img.removeClass("hide")
        $dialog.find(".product-name-user-callback").html(product.Name)
        $dialog.find(".product-raty-text-user-callback").html(reviewText)
        $dialog.find(".product-raty-image-user-callback").raty({
            score: product.Rating,
            showHalf: true,
            path: "../images/rating",
            targetKeep: true,
            precision: true,
            readOnly: true
        })

        $dialog.find(".product-user-callback-review-list").html(templateReviews)

        if (!Reviews || Reviews.length == 0) {
            setEmptyReview()
        }

        //$dialog.trigger("showModal")
        Dialog.showModal($dialog)

        loader.stop()
    }

    loadProductReviews(productId, callback)
}

var Reviews = []

function loadProductReviews(productId, callback) {
    let successFunc = function (result) {
        if (result.Success) {
            if (result.Data &&
                result.Data.length > 0) {
                Reviews = result.Data
            }
        } else {
            showErrorMessage(result.ErrorMessage)
        }

        if (callback) {
            callback()
        }
    }

    $.post("/Admin/LoadProductReviews", { productId: productId }, successCallBack(successFunc))
}

function setEmptyOrders(containerId) {
    containerId = containerId || getCurrentSectionId()
    let template = `
        <div class="empty-list">
            <div class="empty-list-animation">
            <img src="../Images/loader-empty-order.gif"/>
            </div>
            <span>Пока нет ни одного заказа</span>
        </div>
    `

    $(`#${containerId} .order-list-grid`).append(template)
}

function removeEmptyOrders(containerId) {
    containerId = containerId || getCurrentSectionId()
    $(`#${containerId} .order-list-grid .empty-list`).remove()
}

function getCurrentBranchId() {
    return $("#current-brnach").val()
}

var Orders = []
const newOrderToolOptions = {
    containerId: 'new-order-tool',
    orderType: OrderType.NewOrders,
    tools: [
        { type: OrderToolType.OrderAsk, isActive: true },
        { type: OrderToolType.OrderDesc, isActive: false }
    ]
}
function loadOrders(reload = false) {
    if (Orders.length != 0 && !reload) {
        return
    }

    new OrderTools(newOrderToolOptions)

    clearSearchInput(Pages.Order)
    let currentBranchId = getCurrentBranchId()
    let brnachIds = [...AdditionalBranch]
    brnachIds.push(currentBranchId)

    let $list = $("#order .orders .order-list")
    $list.empty()

    let loader = new Loader($("#order"))
    loader.start()

    let successFunc = function (data, loader) {
        if (data.Success) {
            Orders = processingOrders(data.Data.Orders)
            showCountOrder(Orders.length)
            setTodayDataOrders(data.Data.TodayData, Pages.Order)

            if (Orders.length == 0) {
                setEmptyOrders(Pages.Order)
            } else {
                CardOrderRenderer.renderOrders(Orders, Pages.Order, 600)
            }
        } else {
            showErrorMessage(data.ErrorMessage)
        }

        loader.stop()
    }

    $.post("/Admin/LoadOrders", { brnachIds: brnachIds }, successCallBack(successFunc, loader))
}

var HistoryOrders = []

function clearOrdersContainer(containerId) {
    $(`#${containerId} .order-list-grid`).empty()
}

const historyOrderToolOptions = {
    containerId: 'history-order-tool',
    orderType: OrderType.HistoryOrders,
    tools: [
        { type: OrderToolType.OrderAsk, isActive: true },
        { type: OrderToolType.OrderDesc, isActive: false },
        { type: OrderToolType.OrderApply, isActive: true },
        { type: OrderToolType.OrderCancel, isActive: true }
    ]
}

function loadHistoryOrders() {
    clearSearchInput(Pages.HistoryOrder)
    currentBranchId = getCurrentBranchId()
    let brnachIds = [...AdditionalHistoryBranch]

    brnachIds.push(currentBranchId)

    new OrderTools(historyOrderToolOptions)

    let $list = $("#history .orders .order-list")
    $list.empty()

    let loader = new Loader($("#history"))
    loader.start()

    let successFunc = function (data, loader) {
        if (data.Success) {
            HistoryOrders = processingOrders(data.Data)
            clearOrdersContainer(Pages.HistoryOrder)

            if (HistoryOrders.length == 0) {
                setEmptyOrders(Pages.HistoryOrder)
            } else {
                removeEmptyOrders(Pages.HistoryOrder)
                CardOrderRenderer.renderOrders(HistoryOrders, Pages.HistoryOrder, 600)
            }

            changeHistoryOrderDataForStatusBar()
        } else {
            showErrorMessage(data.ErrorMessage)
        }

        loader.stop()
    }

    $.post("/Admin/LoadHistoryOrders", {
        BranchIds: brnachIds,
        StartDate: OrderHistoryDatePicker.minRange.toJSON(),
        EndDate: OrderHistoryDatePicker.maxRange.toJSON(),

    }, successCallBack(successFunc, loader))
}

function jsonToDate(value) {
    let date
    if (value.includes("/Date")) {
        date = new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10))
    } else {
        date = new Date(value)
    }

    return date
}

function processingOrders(orders) {
    for (let order of orders) {
        processsingOrder(order)
    }

    return orders
}

function processsingOrder(order) {
    order.Date = jsonToDate(order.Date)
    order.DateDelivery = order.DateDelivery ? jsonToDate(order.DateDelivery) : null
    order.ProductCount = JSON.parse(order.ProductCountJSON)
    order.ProductBonusCount = JSON.parse(order.ProductBonusCountJSON)
    order.ProductConstructorCount = JSON.parse(order.ProductConstructorCountJSON)
    order.ProductWithOptionsCount = JSON.parse(order.ProductWithOptionsCountJSON)

    return order
}

function toStringDateAndTime(date) {
    date = new Date(date)
    let day = date.getDate().toString()
    day = day.length == 1 ? "0" + day : day
    let month = (date.getMonth() + 1).toString()
    month = month.length == 1 ? "0" + month : month
    let hours = date.getHours().toString()
    hours = hours.length == 1 ? "0" + hours : hours
    let minutes = date.getMinutes().toString()
    minutes = minutes.length == 1 ? "0" + minutes : minutes
    let dateStr = `${hours}:${minutes} ${day}.${month}.${date.getFullYear()}`
    return dateStr
}


function toStringDate(date) {
    let day = date.getDate().toString()
    day = day.length == 1 ? "0" + day : day
    let month = (date.getMonth() + 1).toString()
    month = month.length == 1 ? "0" + month : month

    let dateStr = `${day}.${month}.${date.getFullYear()}`
    return dateStr
}

function getParentItemFromOrdersDOM(e) {
    let $e = $(e)
    let cssClass = "order-item"
    let $parent = $e.parents(`.${cssClass}`)

    if ($parent.length == 0 &&
        $e.hasClass(cssClass)) {
        $parent = $e
    }

    return $parent
}

function getOrderIdFromItemOrdersDOM(e) {
    $parent = getParentItemFromOrdersDOM(e)

    return $parent.attr("order-id")
}

function getItemsIdsForLoad(order) {
    let productIds = []
    let constructorCategoryIds = []

    for (let id in order.ProductCount) {
        productIds.push(id)
    }

    if (order.ProductBonusCount && Object.keys(order.ProductBonusCount).length > 0) {
        for (let id in order.ProductBonusCount) {
            productIds.push(id)
        }
    }

    if (order.ProductConstructorCount && order.ProductConstructorCount.length > 0) {
        for (let productConstructorOrder of order.ProductConstructorCount) {
            constructorCategoryIds.push(productConstructorOrder.CategoryId)
        }
    }

    if (order.ProductWithOptionsCount && order.ProductWithOptionsCount.length > 0) {
        for (let productWithOptions of order.ProductWithOptionsCount) {
            productIds.push(productWithOptions.ProductId)
        }
    }

    return { productIds, constructorCategoryIds }
}

function isInteger(num) {
    return (num ^ 0) === num
}

function getPriceValid(num) {
    if (!isInteger(num)) {
        num = num.toFixed(2)
    }

    return num
}

var OrderProducts = {}
var OrderConstructorProducts = {}
function loadItemForOrder(idsCollection, callback) {
    $.post("/Admin/LoadOrderItems", { productIds: idsCollection.productIds, constructorCategoryIds: idsCollection.constructorCategoryIds }, successCallBack(callback, null))
}

function getOrderById(orderId) {
    return baseGetOrderByIrd(Orders, orderId)
}

function baseGetOrderByIrd(collection, orderId) {
    let searchOrder = null

    for (let order of collection) {
        if (order.Id == orderId) {
            searchOrder = order
            break
        }
    }

    return searchOrder
}

function getHistoryOrderById(orderId) {
    return baseGetOrderByIrd(HistoryOrders, orderId)
}

function getCityNameById(id) {
    let $cityItem = $(`#setting-city-list [city-id=${id}]`)
    let cityName = ""

    if ($cityItem.length != 0) {
        cityName = $cityItem.val()
    }

    return cityName
}

var BuyType = {
    Cash: 1,
    Card: 2,
    Online: 3,
}

function getBuyType(id) {
    let buyType = ""

    switch (id) {
        case BuyType.Cash:
            buyType = "Наличные"
            break
        case BuyType.Card:
            buyType = "Банковская карта"
            break
        case BuyType.Online:
            buyType = "Онлайн"
            break
    }

    return buyType
}

var DeliveryType = {
    TakeYourSelf: 1,
    Delivery: 2
}

function getDeliveryType(id) {
    let deliveryType = ""

    switch (id) {
        case DeliveryType.TakeYourSelf:
            deliveryType = "Самовывоз"
            break
        case DeliveryType.Delivery:
            deliveryType = "Доставка курьером"
            break
    }

    return deliveryType
}

const OrderStatus = {
    Processing: 0,
    Processed: 1,
    Cancellation: 2
}

function changeOrderStatus(orderId, orderStatus, commentCauseCancel) {
    let $order = $(`#order .order-list-grid [order-id=${orderId}]`)

    $order.hide(500, function () {
        $(this.remove())
        for (let i = 0; i < Orders.length; ++i) {
            if (Orders[i].Id == orderId) {
                Orders.splice(i, 1)
                break
            }
        }

        showCountOrder(Orders.length)

        if (Orders.length == 0) {
            setEmptyOrders()
        }
    })

    $.post("/Admin/UpdateStatusOrder", { OrderId: orderId, Status: orderStatus, CommentCauseCancel: commentCauseCancel }, successCallBack(() => getTodayDataOrders(Pages.Order)))
}

function searchByOrderNumber(containerId, isAnimation = true) {
    let searchNumber = $(`#${containerId} .search-input input`).val()
    $(`#${containerId} .order-item-grid`).each(function (index, e) {
        let $e = $(e)
        let orderNumber = $e.attr("order-id")

        if (orderNumber.includes(searchNumber)) {
            if (isAnimation)
                $e.show(500)
            else
                $e.show()
        } else {
            if (isAnimation)
                $e.hide(500)
            else
                $e.hide()
        }
    })

    if (containerId == Pages.HistoryOrder) {
        changeHistoryOrderDataForStatusBar()
    }
}

function showCountOrder(count) {
    let $orderCount = $(".order-count")
    if (count) {
        $orderCount.removeClass("hide")
        $orderCount.html(` (<span>+${count}</span>)`)
    } else {
        $orderCount.addClass("hide")
    }

    OrderStatusBar.setCountNewOrder(count, Pages.Order)
}

function initNotifySoundNewOrder() {
    let notifySoundNewOrder = new Audio('../Content/sounds/sound-new-order.mp3')
    notifySoundNewOrder.autoplay = false

    return notifySoundNewOrder
}

var NotifySoundNewOrder = initNotifySoundNewOrder()

NotifySoundNewOrder.stop = function () {
    if (!NotifySoundNewOrder.paused) {
        NotifySoundNewOrder.pause()
        NotifySoundNewOrder.currentTime = 0
    }
}

function notifySoundNewOrder(isRepead = false) {
    let soundOn = $("#sound-nodify").is(":checked")

    if (soundOn) {
        try {
            NotifySoundNewOrder.stop()
            NotifySoundNewOrder.play()
        } catch {
            logconsole.error('Error notifySoundNewOrder')

            if (!isRepead) {
                NotifySoundNewOrder = initNotifySoundNewOrder()
                notifySoundNewOrder(true)
            }
        }


    }
}

function getTodayDataOrders(containerId) {
    let currentBranchId = getCurrentBranchId()
    let brnachIds = [...AdditionalBranch]
    brnachIds.push(currentBranchId)

    let successFunct = data => {
        setTodayDataOrders(data.Data, containerId)
    }

    $.post("/Admin/GetTodayOrderData", { brnachIds: brnachIds }, successCallBack(successFunct))
}

function changeHistoryOrderDataForStatusBar() {
    let historyOrderData = {
        CountSuccesOrder: 0,
        CountCancelOrder: 0,
        Revenue: 0
    }

    HistoryOrders.forEach(v => {
        if (v.OrderStatus == OrderStatus.Processed) {
            ++historyOrderData.CountSuccesOrder
            historyOrderData.Revenue += v.AmountPayDiscountDelivery
        } else {
            ++historyOrderData.CountCancelOrder
        }
    })

    setTodayDataOrders(historyOrderData, Pages.HistoryOrder)
}

function setTodayDataOrders(data, containerId) {
    OrderStatusBar.setCountSuccessOrder(data.CountSuccesOrder, containerId)
    OrderStatusBar.setCountCancelOrder(data.CountCancelOrder, containerId)
    OrderStatusBar.setTodayRevenue(data.Revenue, containerId)
}

function getCurrentSectionId() {
    return $(".section:not(.hide)").attr("id")
}

class OrderStatusBar {

    static setCountNewOrder(value, containerId) {
        this.setValue(".count-new-order", value, containerId)
    }

    static setCountSuccessOrder(value, containerId) {
        this.setValue(".count-success-order", value, containerId)
    }

    static setCountCancelOrder(value, containerId) {
        this.setValue(".count-cancel-order", value, containerId)
    }

    static setTodayRevenue(value, containerId) {
        this.setValue(".today-revenue-order", getPriceValid(value), containerId)
    }

    static setValue(qSelector, value, containerId) {
        const selectorValueContainer = ".status-bar-value"
        $(`#${containerId} ${qSelector} ${selectorValueContainer}`).html(value)
    }
}

function convertIngredientsToDictionary(ingredients) {
    let dict = {}

    for (ingredient of ingredients) {
        dict[ingredient.Id] = ingredient
    }

    return dict
}

function showOrderDetailsFromPromotionClient(order) {
    const currentSectionId = getCurrentSectionId()
    const dialogId = order.orderStatus == OrderStatus.Processing ? 'orderDetailsDialog' : 'historyOrderDetailsDialog'
    const getOrderDetails = () => {
        const orderDetailsData = new OrderDetailsData(order)

        return new OrderDetails(orderDetailsData, dialogId)
    }


    let loader = new Loader($(`#${currentSectionId}`))
    loader.start()

    let callbackLoadProducts = function (data) {
        if (data.Success) {
            for (let categoryObj of data.Data.products) {

                categoryObj.Products.forEach(product => OrderProducts[product.Id] = product)
            }

            for (let categoryObj of data.Data.constructor) {
                categoryObj.Ingredients = convertIngredientsToDictionary(categoryObj.Ingredients)
                OrderConstructorProducts[categoryObj.CategoryId] = categoryObj
            }

            loader.stop()
            getOrderDetails().show()
        } else {
            loader.stop()
            showErrorMessage(data.ErrorMessage)
        }
    }

    let itemsIdsforLoad = getItemsIdsForLoad(order)

    if (itemsIdsforLoad.productIds.length == 0 && itemsIdsforLoad.constructorCategoryIds.length == 0) {
        loader.stop()
        getOrderDetails().show()
    } else {
        loadItemForOrder(itemsIdsforLoad, callbackLoadProducts)
    }
}

function showOrderDetails(orderId) {
    const currentSectionId = getCurrentSectionId()
    const order = currentSectionId == Pages.HistoryOrder ? getHistoryOrderById(orderId) : getOrderById(orderId)
    const dialogId = currentSectionId == Pages.HistoryOrder ? 'historyOrderDetailsDialog' : 'orderDetailsDialog'
    const getOrderDetails = () => {
        const orderDetailsData = new OrderDetailsData(order)

        return new OrderDetails(orderDetailsData, dialogId)
    }


    let loader = new Loader($(`#${currentSectionId}`))
    loader.start()

    let callbackLoadProducts = function (data) {
        if (data.Success) {
            for (let categoryObj of data.Data.products) {

                categoryObj.Products.forEach(product => OrderProducts[product.Id] = product)
            }

            for (let categoryObj of data.Data.constructor) {
                categoryObj.Ingredients = convertIngredientsToDictionary(categoryObj.Ingredients)
                OrderConstructorProducts[categoryObj.CategoryId] = categoryObj
            }

            loader.stop()
            getOrderDetails().show()
        } else {
            loader.stop()
            showErrorMessage(data.ErrorMessage)
        }
    }

    let itemsIdsforLoad = getItemsIdsForLoad(order)

    if (itemsIdsforLoad.productIds.length == 0 && itemsIdsforLoad.constructorCategoryIds.length == 0) {
        loader.stop()
        getOrderDetails().show()
    } else {
        loadItemForOrder(itemsIdsforLoad, callbackLoadProducts)
    }
}

var Dialog = {
    transition: null,
    showModal: ($dialog) => {
        $dialog = $($dialog)

        $dialog.trigger("showModal")
        this.transition = setTimeout(function () {
            $dialog.addClass('dialog-scale')
        }, 0.5)
    },
    close: ($dialog) => {
        $dialog = $($dialog)

        $dialog.trigger("close")
        $dialog.removeClass('dialog-scale')
        clearTimeout(this.transition)
    },
    clear: ($dialog) => {
        $dialog = $($dialog)

        $dialog.find("input").val("")
        $dialog.find("input[type=checkbox]").prop('checked', false)
        $dialog.find("textarea").val("")
        $dialog.find(".dialog-image-upload").removeClass("hide")
        $dialog.find("img").not('.no-clean').removeAttr("src")
        $dialog.find("img").not('.no-clean').addClass("hide")
        $dialog.find("option").removeAttr("selected")
        $dialog.find("select").val("0")
        $dialog.find('#btn-function-product-additional-info').attr('disabled', true)
        $dialog.find('#btn-product-options').attr('disabled', true)
        $dialog.find('#product-additional-info').attr('type', 'text')
        const $select = $dialog.find("select")
        $select.each(function () {
            const $e = $(this)
            if ($e[0].sumo) {
                $e[0].sumo.enable()
                $e[0].sumo.reload()
            }
        })
    }
}

class CardOrder {
    /**
     *
     * @param {TodayOrder} cardData
     */
    constructor(cardData) {
        const templateId = "order-item-grid-template"
        this.data = cardData
        this.$htmlTemplate = $($(`#${templateId}`).html())
    }

    setOrderAttres(orderId, statusId) {
        this.$htmlTemplate.attr("order-id", orderId)

        if (statusId != StatusAtrr.Processing)
            this.$htmlTemplate.attr("status-id", statusId)
    }

    setOrderNumber(value, status) {
        this.setValue(".order-item-number", value)
        this.setValue(".preorder-item-grid .order-item-number", value)
        this.markNumberOrder(".order-item-number", status)
    }

    setAmount(value) {
        this.setValue(".order-item-amount", value)
        this.setValue(".preorder-item-grid .order-item-amount", value)
    }

    setDateDeliveryPreoprder(value) {
        this.setValue(".preorder-item-grid .preorder-date-delivery", value)
    }

    setPhoneNumber(value) {
        this.setValue(".order-item-phone", value)
    }

    setUserName(value) {
        this.setValue(".order-item-user-name", value)
    }

    setDeliverType(value) {
        this.setValue(".order-item-type-delivery", value)
    }

    setPayType(value) {
        this.setValue(".order-item-type-pay", value)
    }

    setValue(qSelector, value) {
        const selectorValueContainer = ".order-item-value"
        this.$htmlTemplate.find(`${qSelector} ${selectorValueContainer}`).html(value)
    }

    setDetailsClickAction(action) {
        const eSenderContainer = ".order-item-show-details"
        const $eSender = $(this.$htmlTemplate.find(`${eSenderContainer} a`))

        $eSender.unbind("click")
        $eSender.bind("click", action)
    }

    markNumberOrder(qSelector, status) {
        const selectorLabelContainer = ".order-item-label"
        let label = ""

        switch (status) {
            case OrderStatus.Processed:
                label = StatusAtrr.Processed.numberOrderMark
                break
            case OrderStatus.Cancellation:
                label = StatusAtrr.Cancellation.numberOrderMark
                break
            default:
                label = StatusAtrr.Processing.numberOrderMark
                break
        }

        this.$htmlTemplate.find(`${qSelector} ${selectorLabelContainer}`).html(label)
    }

    setPreorderViewMask() {
        const templateId = "preorder-item-grid-template"
        let htmlTemplatePreorderMask = $(`#${templateId}`).html()

        this.$htmlTemplate.append(htmlTemplatePreorderMask)
    }

    render() {
        if (this.data.IsPreorder) {
            this.setPreorderViewMask()
            this.setDateDeliveryPreoprder(this.data.DateDelivery)
        }

        this.setOrderAttres(this.data.OrderId, this.data.Status)
        this.setOrderNumber(this.data.OrderNumber, this.data.Status)
        this.setAmount(this.data.Amount)
        this.setPhoneNumber(this.data.PhoneNumber)
        this.setUserName(this.data.UserName)
        this.setDeliverType(this.data.DeliveryType)
        this.setPayType(this.data.PayType)
        this.setDetailsClickAction(this.data.Action)

        return this.$htmlTemplate
    }
}

class TodayOrder {
    constructor(order, action) {
        this.Action = () => { action(order.Id) }

        this.convert(order)
    }

    convert(order) {
        this.OrderId = order.Id
        this.Status = order.OrderStatus
        this.OrderNumber = order.Id
        this.Amount = xFormatPrice(order.AmountPayDiscountDelivery)
        this.PhoneNumber = order.PhoneNumber
        this.UserName = order.Name
        this.DeliveryType = getDeliveryType(order.DeliveryType)
        this.PayType = getBuyType(order.BuyType)
        this.IsPreorder = this.isPreoprder(order.DateDelivery)
        this.DateDelivery = this.IsPreorder ? toStringDate(order.DateDelivery) : null
    }

    isPreoprder(dateDelivery) {
        let isPreorder = false

        if (dateDelivery) {
            let dateNow = new Date()
            dateNow.setHours(0, 0, 0, 0)

            let dateDeliveryCopy = new Date(dateDelivery)
            dateDeliveryCopy.setHours(0, 0, 0, 0)

            isPreorder = dateNow.toDateString() != dateDeliveryCopy.toDateString() &&
                dateDelivery > new Date()
        }

        return isPreorder
    }
}

class CardOrderRenderer {
    static renderOrders(orders, containerId, speed) {
        let index = 0
        for (let order of orders) {
            ++index

            if (index > 6) {
                speed = 1
            }

            this.renderOrder(order, containerId, speed)
        }
    }

    static renderOrder(order, containerId, speed) {
        const todayData = new TodayOrder(order, showOrderDetails)
        const cardOrder = new CardOrder(todayData)

        this.addCardToPage(cardOrder, containerId, speed)
    }

    static addCardToPage(card, containerId, speed) {
        const cardContainer = ".order-list-grid"
        const cardRender = card.render()
        const isAppnd = $(`#${containerId} .order-tools button[order-type=${OrderToolType.OrderAsk}]`)
            .hasClass('active-tool-item')
        const $cardContainer = $(`#${containerId} ${cardContainer}`)

        if (isAppnd)
            $cardContainer.append(cardRender)
        else
            $cardContainer.prepend(cardRender)

        this.showCard(cardRender, speed)
    }

    static showCard(cardRender, speed) {
        if (speed == 1)
            $(cardRender).show()
        else
            $(cardRender).show(speed || 500)
    }
}

class OrderDetailsData {
    constructor(order) {
        this.convert(order)
    }

    convert(order) {
        this.convertBaseInfo(order)
        this.convertShortInfo(order)
        this.convertAmountInfo(order)
        this.convertAddressInfo(order)
        this.converеOrderListInfo(order)
        this.setPermissionsSendToIntegrationSystem(order)
    }

    convertBaseInfo(order) {
        const commentEmptyTemplate = `<i class="fal fa-comment-slash"></i>Отсутсвтует</span>`
        this.OrderId = order.Id
        this.OrderNumber = order.Id
        this.NumberAppliances = order.NumberAppliances
        this.Status = order.OrderStatus
        this.CauseCancelOrderComment = order.CommentCauseCancel || ''
        this.OrderDate = toStringDateAndTime(order.Date)
        this.OrderDeliveryDate = order.DateDelivery ?
            toStringDateAndTime(order.DateDelivery) :
            'Как можно быстрее'
        this.Comment = order.Comment || commentEmptyTemplate
    }

    convertShortInfo(order) {
        this.UserName = order.Name
        this.PhoneNumber = order.PhoneNumber
        this.DeliveryType = getDeliveryType(order.DeliveryType)
    }

    getDiscounText(order) {
        const prefixRub = "руб."
        const prefixPercent = "%"
        const percent = order.DiscountPercent == 0 ? `0${prefixPercent}` : `${order.DiscountPercent}${prefixPercent} (${xFormatPrice(order.AmountPay * order.DiscountPercent / 100)} ${prefixRub})`

        const ruble = order.DiscountRuble > 0 ? `${order.DiscountRuble} руб.` : ''
        let text = percent && ruble ? `${percent} и ${ruble}` : percent || ruble

        return text
    }

    convertAmountInfo(order) {
        const prefixRub = "руб."
        const prefixPercent = "%"

        this.AmountPay = `${xFormatPrice(order.AmountPay)} ${prefixRub}`
        this.AmountPayCashBack = `${xFormatPrice(order.AmountPayCashBack)} ${prefixRub}`
        this.DeliveryPrice = `${xFormatPrice(order.DeliveryPrice)} ${prefixRub}`
        this.Discount = this.getDiscounText(order)
        this.PayType = getBuyType(order.BuyType)
        this.CashBack = order.CashBack > 0 ?
            `${xFormatPrice(order.CashBack - order.AmountPayDiscountDelivery)} ${prefixRub} (c ${xFormatPrice(order.CashBack)} ${prefixRub})` :
            `${xFormatPrice(order.CashBack)} ${prefixRub}`
        const markPay = order.BuyType == BuyType.Online ? 'Оплачено ' : ''
        this.AmountPayDiscountDelivery = `${markPay}${xFormatPrice(order.AmountPayDiscountDelivery)} ${prefixRub}`
    }

    convertAddressInfo(order) {
        this.City = getCityNameById(order.CityId)
        this.Street = order.DeliveryType == DeliveryType.Delivery ? order.Street : $("#setting-street").val()
        this.House = order.DeliveryType == DeliveryType.Delivery ? order.HomeNumber : $("#setting-home").val()
        this.Apartament = order.DeliveryType == DeliveryType.Delivery ? order.ApartamentNumber : "-"
        this.Level = order.DeliveryType == DeliveryType.Delivery ? order.Level : "-"
        this.IntercomCode = order.DeliveryType == DeliveryType.Delivery ? order.IntercomCode : "-"
        this.Entrance = order.DeliveryType == DeliveryType.Delivery ? order.EntranceNumber : "-"
    }

    converеOrderListInfo(order) {
        const prefixRub = "руб."
        this.OrderList = []
        this.OrderWithOptionsList = []
        this.OrderProductConstructorList = []



        if (order.ProductCount && Object.keys(order.ProductCount).length > 0) {
            for (let productId in order.ProductCount) {
                const product = OrderProducts[productId]
                const obj = {
                    Image: product.Image,
                    Name: product.Name,
                    Price: `${order.ProductCount[productId]} x ${product.Price} ${prefixRub}`
                }

                const view = this.convertOrderProducrToView(obj)
                this.OrderList.push(view)
            }
        }

        if (order.ProductBonusCount && Object.keys(order.ProductBonusCount).length > 0) {
            for (let productId in order.ProductBonusCount) {
                const product = OrderProducts[productId]
                let obj = {}

                if (product.ProductAdditionalOptionIds.length > 0) {
                    const options = []
                    for (const id of product.ProductAdditionalOptionIds) {
                        const additionalOption = DataProduct.AdditionalOptions[id]
                        const additoinalOptionItem = additionalOption.Items.find(p => p.IsDefault)

                        options.push({ name: additoinalOptionItem.Name, priceStr: `0 ${prefixRub}` })
                    }

                    obj = {
                        Image: product.Image,
                        Name: product.Name,
                        Price: `${order.ProductBonusCount[productId]} x 0 ${prefixRub}`,
                        Options: options
                    }

                    const view = this.convertOrderProductWithOptionsToView(obj, true)
                    this.OrderWithOptionsList.push(view)
                } else {
                    obj = {
                        Image: product.Image,
                        Name: product.Name,
                        Price: `${order.ProductBonusCount[productId]} x 0 ${prefixRub}`
                    }

                    const view = this.convertOrderProducrToView(obj, true)
                    this.OrderList.push(view)
                }
            }
        }

        if (order.ProductConstructorCount && order.ProductConstructorCount.length > 0) {
            for (let constructorItem of order.ProductConstructorCount) {
                let contructor = OrderConstructorProducts[constructorItem.CategoryId]
                let productConstructorData = this.getProductConstructorData(constructorItem.IngredientCount, contructor.Ingredients)
                let constructorToView = {
                    Image: contructor.CategoryImage,
                    Name: contructor.CategoryName,
                    Price: `${constructorItem.Count} x ${productConstructorData.price} ${prefixRub}`,
                    Ingredients: productConstructorData.ingredients
                }
                const view = this.convertOrderConstructorProducrToView(constructorToView)
                this.OrderProductConstructorList.push(view)
            }
        }

        if (order.ProductWithOptionsCount && order.ProductWithOptionsCount.length > 0) {
            const getProductWithOptionsData = (product, orderProductWithOptions) => {
                let price = product.Price
                const options = []

                if (Object.keys(orderProductWithOptions.AdditionalOptions).length) {
                    for (const id in orderProductWithOptions.AdditionalOptions) {
                        const additionalOption = DataProduct.AdditionalOptions[id]
                        const additionalOptionItemId = orderProductWithOptions.AdditionalOptions[id]
                        const additoinalOptionItem = additionalOption.Items.find(p => p.Id == additionalOptionItemId)

                        price += additoinalOptionItem.Price
                        options.push({ name: additoinalOptionItem.Name, priceStr: `${additoinalOptionItem.Price} ${prefixRub}` })
                    }
                }

                if (orderProductWithOptions.AdditionalFillings.length > 0) {
                    for (const id of orderProductWithOptions.AdditionalFillings) {
                        const additionalFilling = DataProduct.AdditionalFillings[id]

                        price += additionalFilling.Price
                        options.push({ name: additionalFilling.Name, priceStr: `${additionalFilling.Price} ${prefixRub}` })
                    }
                }

                return { price, options }
            }

            for (let orderProductWithOptions of order.ProductWithOptionsCount) {
                const product = OrderProducts[orderProductWithOptions.ProductId]
                const productOptionsData = getProductWithOptionsData(product, orderProductWithOptions)
                const obj = {
                    Image: product.Image,
                    Name: product.Name,
                    Price: `${orderProductWithOptions.Count} x ${productOptionsData.price} ${prefixRub}`,
                    Options: productOptionsData.options
                }

                const view = this.convertOrderProductWithOptionsToView(obj)
                this.OrderWithOptionsList.push(view)
            }
        }
    }

    getProductConstructorData(constructorItem, ingredients) {
        const prefixRub = "руб."
        let price = 0
        let ingredientList = []
        for (let id in constructorItem) {
            const count = constructorItem[id]
            const ingredient = ingredients[id]
            ingredientList.push({
                Image: ingredient.Image,
                Name: ingredient.Name,
                Price: `${count} x ${ingredient.Price} ${prefixRub}`
            })
            price += (ingredient.Price * count)
        }

        return {
            price,
            ingredients: ingredientList
        }
    }

    convertOrderProducrToView(product, isBonusProduct = false) {
        const cssClassBonusProduct = isBonusProduct ? 'order-details-product-bonus' : ''
        return `
            <div class="order-details-product-item ">
                <div class="order-details-product-img">
                    <img src="${product.Image}">
                </div>
                <div class="order-details-product-name-price border-bottom ${cssClassBonusProduct}">
                    <span>${product.Name}</span>
                    <span class="font-weight-bold grid-justify-self-flex-end">${product.Price}</span>
                </div>
             </div>
        `
    }

    convertOrderConstructorProducrToView(constructor) {
        return `
            <div class="order-details-constructor-product-item">
                <div class="order-details-constructor-product-header">
                    <div class="order-details-constructor-product-img">
                        <img src="${constructor.Image}">
                    </div>
                    <div class="order-details-constructor-product-name-price">
                        <span>${constructor.Name}</span>
                        <span class="font-weight-bold grid-justify-self-flex-end">${constructor.Price}</span>
                    </div>
                </div>
                <div class="order-details-constructor-product-ingredients border-bottom">
                    ${this.getIngredientViews(constructor.Ingredients)}
                </div>
             </div>
        `
    }

    convertOrderProductWithOptionsToView(constructor, isBonusProduct = false) {
        const cssClassBonusProduct = isBonusProduct ? 'order-details-product-bonus' : ''

        return `
            <div class="order-details-constructor-product-item">
                <div class="order-details-constructor-product-header">
                    <div class="order-details-constructor-product-img">
                        <img src="${constructor.Image}">
                    </div>
                    <div class="order-details-constructor-product-name-price ${cssClassBonusProduct}">
                        <span>${constructor.Name}</span>
                        <span class="font-weight-bold grid-justify-self-flex-end">${constructor.Price}</span>
                    </div>
                </div>
                <div class="order-details-constructor-product-ingredients border-bottom">
                    ${this.getOptionViews(constructor.Options)}
                </div>
             </div>
        `
    }

    getOptionViews(options) {
        let views = ''

        for (let option of options) {
            views += this.getOptionView(option)
        }

        return views
    }

    getOptionView(option) {
        return `
            <div class="order-product-option-header">
                <div class="order-details-constructor-product-name-price">
                    <span>${option.name}</span>
                    <span class="font-weight-bold grid-justify-self-flex-end">${option.priceStr}</span>
                </div>
             </div>
        `
    }

    getIngredientViews(ingredients) {
        let views = ''

        for (let ingredient of ingredients) {
            views += this.getIngredientView(ingredient)
        }

        return views
    }

    getIngredientView(ingredient) {
        return `
            <div class="order-details-constructor-product-header">
                <div class="order-details-constructor-ingredient-img">
                    <img src="${ingredient.Image}">
                </div>
                <div class="order-details-constructor-product-name-price">
                    <span>${ingredient.Name}</span>
                    <span class="font-weight-bold grid-justify-self-flex-end">${ingredient.Price}</span>
                </div>
             </div>
        `
    }

    setPermissionsSendToIntegrationSystem(order) {
        let allowedSendToIntegrationSystem = true

        if (!order.IsSendToIntegrationSystem) {
            if (order.ProductCount && Object.keys(order.ProductCount).length > 0) {
                for (let productId in order.ProductCount) {
                    const product = OrderProducts[productId]

                    if (!product.VendorCode) {
                        allowedSendToIntegrationSystem = false
                        break
                    }
                }
            }

            if (allowedSendToIntegrationSystem && order.ProductBonusCount && Object.keys(order.ProductBonusCount).length > 0) {
                for (let productId in order.ProductBonusCount) {
                    const product = OrderProducts[productId]

                    if (!product.VendorCode) {
                        allowedSendToIntegrationSystem = false
                        break
                    }
                }
            }

            allowedSendToIntegrationSystem = allowedSendToIntegrationSystem && !(order.ProductConstructorCount && order.ProductConstructorCount.length > 0)
            allowedSendToIntegrationSystem = allowedSendToIntegrationSystem && !(order.ProductWithOptionsCount && order.ProductWithOptionsCount.length > 0)
            allowedSendToIntegrationSystem = allowedSendToIntegrationSystem && !(order.DiscountPercent != 0 && order.DiscountRuble !=0)
        } else
            allowedSendToIntegrationSystem = false

        this.AllowedSendToIntegrationSystem = allowedSendToIntegrationSystem
        this.IsSendToIntegrationSystem = order.IsSendToIntegrationSystem
        this.IntegrationOrderNumber = `#${order.IntegrationOrderNumber}`
    }
}

var OrderDetailsQSelector = {
    OrderNumberBlock: ".order-details-order-number-cutlery .order-details-number",//в истории заказов помечаем цветом оформленный или не оформленный ордер
    OrderNumber: ".order-details-number .value",
    NumberAppliances: ".order-details-number-appliances .value",
    OrderDate: ".order-details-date .value",
    OrderDeliveryDate: ".order-details-date-delivery .value",
    UserName: ".order-details-short-user-name .value",
    PhoneNumber: ".order-details-short-phone .value",
    DeliveryType: ".order-details-short-delivery-type .value",
    PriceAmount: ".order-details-price-amount .value",
    PriceAmountCashbackBonus: ".order-details-price-amount-casback-bonus .value",
    DeliveryPrice: ".order-details-price-delivery .value",
    Discount: ".order-details-price-discount .value",
    PayType: ".order-details-price-pay-type .value",
    CashBack: ".order-details-price-cashback .value",
    PriceToPay: ".order-details-price-to-pay .value",
    City: ".order-details-address-city .value",
    Street: ".order-details-address-street .value",
    House: ".order-details-address-house .value",
    Apartament: ".order-details-address-apartment .value",
    Level: ".order-details-address-level .value",
    IntercomCode: ".order-details-address-intercom-code .value",
    Entrance: ".order-details-address-entrance .value",
    OrderList: ".order-details-product-list",
    Comment: ".order-details-comment .value",
    ApplyBtn: ".order-details-menu .btn-details-apply",
    CancelBtn: ".order-details-menu .btn-details-cancel",
    CauseCancelBtn: ".order-details-menu .btn-details-cause-comment",
    SendToIntegtationSystem: "#send-to-integration",
    IntegrationOrderNumber: '#integration-order-number',
    SendToIntegtationSystemLoader: "#send-to-integration-loader",
}

var StatusAtrr = {
    Processed: {
        cssColorClass: "success-color",
        numberOrderMark: `<i class="fal fa-check-double sm-font-size"></i>`
    },
    Cancellation: {
        cssColorClass: "fail-color",
        numberOrderMark: `<i class="fal fa-trash-alt sm-font-size"></i>`
    },
    Processing: {
        cssColorClass: "default-color",
        numberOrderMark: `#`
    }
}

class OrderDetails {
    /**
     *
     * @param {OrderDetailsData} details
     */
    constructor(details, dialogId) {
        this.$dialog = $(`#${dialogId}`)
        this.details = details
    }

    show() {
        this.setValues()
        this.buttonsConfig()
        this.toggleSendToIntegrationBtn()
        this.toggleOrderNumberInIntegrationSystem()

        Dialog.showModal(this.$dialog)
    }

    close() {
        Dialog.close(this.$dialog)
    }

    setValue(qSelectror, value) {
        this.$dialog.find(qSelectror).html(value)
    }

    appendValue(qSelectror, value) {
        this.$dialog.find(qSelectror).append(value)
    }

    setValues() {
        this.setBaseInfo()
        this.setDateAndDeliveryDateInfo()
        this.setShortInfo()
        this.setAmountInfo()
        this.setAddressInfo()
        this.setOrderListInfo()
        this.setComment()
    }

    markOrderNumberColorStatus(status) {
        const $block = this.$dialog.find(OrderDetailsQSelector.OrderNumberBlock)
        let colorClass = ""

        $block.removeClass(StatusAtrr.Processed.cssColorClass)
        $block.removeClass(StatusAtrr.Cancellation.cssColorClass)
        $block.removeClass(StatusAtrr.Processing.cssColorClass)

        switch (status) {
            case OrderStatus.Processed:
                colorClass = StatusAtrr.Processed.cssColorClass
                break
            case OrderStatus.Cancellation:
                colorClass = StatusAtrr.Cancellation.cssColorClass
                break
            default:
                colorClass = StatusAtrr.Processing.cssColorClass
                break
        }

        $block.addClass(colorClass)
    }

    setBaseInfo() {
        this.markOrderNumberColorStatus(this.details.Status)
        this.setValue(OrderDetailsQSelector.OrderNumber, this.details.OrderNumber)
        this.setValue(OrderDetailsQSelector.NumberAppliances, this.details.NumberAppliances)
    }

    setDateAndDeliveryDateInfo() {
        this.setValue(OrderDetailsQSelector.OrderDate, this.details.OrderDate)
        this.setValue(OrderDetailsQSelector.OrderDeliveryDate, this.details.OrderDeliveryDate)
    }

    setShortInfo() {
        this.setValue(OrderDetailsQSelector.UserName, this.details.UserName)
        this.setValue(OrderDetailsQSelector.PhoneNumber, this.details.PhoneNumber)
        this.setValue(OrderDetailsQSelector.DeliveryType, this.details.DeliveryType)
    }

    setAmountInfo() {
        this.setValue(OrderDetailsQSelector.PriceAmount, this.details.AmountPay)
        this.setValue(OrderDetailsQSelector.PriceAmountCashbackBonus, this.details.AmountPayCashBack)
        this.setValue(OrderDetailsQSelector.DeliveryPrice, this.details.DeliveryPrice)
        this.setValue(OrderDetailsQSelector.Discount, this.details.Discount)
        this.setValue(OrderDetailsQSelector.PayType, this.details.PayType)
        this.setValue(OrderDetailsQSelector.CashBack, this.details.CashBack)
        this.setValue(OrderDetailsQSelector.PriceToPay, this.details.AmountPayDiscountDelivery)
    }

    setAddressInfo() {
        this.setValue(OrderDetailsQSelector.City, this.details.City)
        this.setValue(OrderDetailsQSelector.Street, this.details.Street)
        this.setValue(OrderDetailsQSelector.House, this.details.House)
        this.setValue(OrderDetailsQSelector.Apartament, this.details.Apartament)
        this.setValue(OrderDetailsQSelector.Level, this.details.Level)
        this.setValue(OrderDetailsQSelector.IntercomCode, this.details.IntercomCode)
        this.setValue(OrderDetailsQSelector.Entrance, this.details.Entrance)
    }

    setOrderListInfo() {
        this.setValue(OrderDetailsQSelector.OrderList, this.details.OrderList)
        this.appendValue(OrderDetailsQSelector.OrderList, this.details.OrderWithOptionsList)
        this.appendValue(OrderDetailsQSelector.OrderList, this.details.OrderProductConstructorList)
    }

    setComment() {
        this.setValue(OrderDetailsQSelector.Comment, this.details.Comment)
    }

    toggleSendToIntegrationBtn() {
        const $sendToIntegrationBtn = $(OrderDetailsQSelector.SendToIntegtationSystem);

        if (this.details.AllowedSendToIntegrationSystem === true)
            $sendToIntegrationBtn.show()
        else
            $sendToIntegrationBtn.hide()
    }

    toggleOrderNumberInIntegrationSystem() {
        const $integrationOrderNumber = $(OrderDetailsQSelector.IntegrationOrderNumber);

        if (this.details.IsSendToIntegrationSystem === true) {
            $integrationOrderNumber.show()
            $integrationOrderNumber.html(this.details.IntegrationOrderNumber)
        }
        else {
            $integrationOrderNumber.hide()
            $integrationOrderNumber.empty()
        }
    }

    buttonsConfig() {
        const $proccesed = $(OrderDetailsQSelector.ApplyBtn)
        const $cancel = $(OrderDetailsQSelector.CancelBtn)
        const $causeCancelComment = $(OrderDetailsQSelector.CauseCancelBtn)
        const $sendToIntegrationSystem = $(OrderDetailsQSelector.SendToIntegtationSystem)

        const actionOrder = (orderStatus) => () => {
            this.close()
            changeOrderStatus(this.details.OrderId, orderStatus)
        }

        const $inputCommentCauseCancel = $('#cause-comment-cancel-order')
        $inputCommentCauseCancel.val('')

        const actionConfirmCancel = orderStatus => {
            const commentCauseCancel = $inputCommentCauseCancel.val()
            Dialog.close('#confirmOrderCancelDialog')
            this.close()

            changeOrderStatus(this.details.OrderId, orderStatus, commentCauseCancel)
        }

        const actionCancel = (orderStatus) => () => {
            const $btn = $('#cause-comment-cancel-order-btn')
            $btn.unbind('click')
            $btn.click('click', () => actionConfirmCancel(orderStatus))

            Dialog.showModal('#confirmOrderCancelDialog')
        }



        const actionShowCauseCancelComment = () => {
            const $inputHistoryCommentCauseCancel = $('#history-cause-comment-cancel-order')
            $inputHistoryCommentCauseCancel.val(this.details.CauseCancelOrderComment)

            const $btn = $('#history-cause-comment-cancel-order-btn')

            $btn.unbind('click')
            $btn.click('click', () => Dialog.close('#confirmOrderCancelCommentDialog'))

            Dialog.showModal('#confirmOrderCancelCommentDialog')
        }

        $proccesed.unbind("click")
        $cancel.unbind("click")
        $causeCancelComment.unbind("click")
        $sendToIntegrationSystem.unbind('click')

        $causeCancelComment.removeAttr("disabled")
        if (this.details.Status == OrderStatus.Cancellation)
            $causeCancelComment.bind("click", actionShowCauseCancelComment)
        else
            $causeCancelComment.attr("disabled", true)

        if (getCurrentSectionId() == Pages.HistoryOrder) {
            $proccesed.attr("disabled", true)
            $cancel.attr("disabled", true)
        } else {
            $proccesed.removeAttr("disabled")
            $cancel.removeAttr("disabled")
            $proccesed.bind("click", actionOrder(OrderStatus.Processed))
            $cancel.bind("click", actionCancel(OrderStatus.Cancellation))
            $sendToIntegrationSystem.bind('click', () => sendOrderToIntegrationSystem(this.details.OrderId))
        }
    }
}

function openSttingAreaDelivery() {
    const setting = new AreaDeliverySetting()

    setting.render()

    Dialog.showModal('#areaDeliverySettingDialog')
}

class AreaDeliverySetting {
    constructor() {
        this.bindAppendAreaDelivery()
    }

    bindAppendAreaDelivery() {
        const $appendAreaBtn = $('.area-delivery-settings-add')
        const actionClick = () => this.appendNewArea()

        $appendAreaBtn.unbind('click')
        $appendAreaBtn.bind('click', actionClick)
    }

    render() {
        const items = []

        if (AreaDelivery.length == 0) {
            const epmty = this.renderEmpty()
            this.setListContainer(epmty)
        } else {
            const header = this.renderHeaderItem()
            for (let area of AreaDelivery.sort((a, b) => a.NameArea > b.NameArea ? 1 : -1)) {
                let item = this.renderItem(area)

                items.push(item)
            }

            this.setListToPage(header, items)
        }
    }

    setListContainer(header) {
        $('.area-delivery-settings-list-container').html(header)
    }

    setListToPage(header, items) {
        let $settingsList = $('<div class="area-delivery-settings-list default-color"></div>')
        $settingsList.html(items)
        this.setListContainer([header, $settingsList])
    }

    renderEmpty() {
        return `<div class="area-delivery-empty">Добавте районы доставки...</div>`
    }

    renderItem(areaDelivery) {
        const minPriceWithPrefix = `${areaDelivery.MinPrice} руб.`
        const deliveryPriceWithPrefix = `${areaDelivery.DeliveryPrice} руб.`
        const actionEditClick = () => this.showEditDialog(areaDelivery.NameArea, areaDelivery.MinPrice, areaDelivery.DeliveryPrice, areaDelivery.UniqId)
        const actionRemoveClick = () => this.removeAreaDelivery(areaDelivery.UniqId)
        const template = `
            <div class="area-delivery-settings-item border-bottom">
                <span class="area-name">${areaDelivery.NameArea}</span>
                <span class="area-delivery-span">${minPriceWithPrefix}</span>
                <span class="area-delivery-span">${deliveryPriceWithPrefix}</span>
                <button class="area-delivery-settings-btn edit-btn"><i class="fal fa-edit"></i></button>
                <button class="area-delivery-settings-btn remove-btn"><i class="fal fa-trash-alt"></i></button>
            </div>`
        const $item = $(template)

        $item.find('.edit-btn').bind('click', actionEditClick)
        $item.find('.remove-btn').bind('click', actionRemoveClick)

        return $item
    }

    renderHeaderItem() {
        const template = `
            <div class="area-delivery-settings-header border-bottom">
                <span class="area-name">Имя района</span>
                <span class="area-delivery-span">Мин. сумма заказа</span>
                <span class="area-delivery-span">Стоимость доставки</span>
                <span class="area-delivery-span">Действия</span>
            </div>`
        const $item = $(template)

        return $item
    }

    removeAreaDelivery(uniqId) {
        const loader = new Loader('#areaDeliveryEditDialog')
        loader.start()

        let newAreaDelivery = []

        for (let area of AreaDelivery) {
            if (area.UniqId != uniqId) {
                newAreaDelivery.push(area)
            }
        }

        AreaDelivery = newAreaDelivery

        this.render()
        loader.stop()
    }

    appendNewArea() {
        const uniqId = generateRandomString(10)

        this.showEditDialog('', '', '', uniqId)
    }

    showEditDialog(name, minPrice, deliveryPrice, uniqId) {
        $("#area-name").val(name)
        $("#area-price").val(minPrice)
        $("#area-delivery-price").val(deliveryPrice)
        $("#area-uniqId").val(uniqId)

        const actionSaveClick = () => this.saveAreaDelivery()
        const saveBtn = $('#areaDeliveryEditDialog').find('.btn-submit')

        saveBtn.unbind('click')
        saveBtn.bind('click', actionSaveClick)

        Dialog.showModal('#areaDeliveryEditDialog')
    }

    saveAreaDelivery() {
        const loader = new Loader('#areaDeliveryEditDialog')
        loader.start()

        const name = $("#area-name").val()
        const minPrice = $("#area-price").val()
        const deliveryPrice = $("#area-delivery-price").val()
        const uniqId = $("#area-uniqId").val()
        const names = name.split(',').map(p => p.trim()).filter(p => p)

        if (!name || !minPrice || !deliveryPrice) {
            showInfoMessage('Заполните все поля')
            loader.stop()
            return
        }

        if (names.length > 1)
            this.multiSaveAreaDelivery(names, minPrice, deliveryPrice)
        else
            this.singleSaveAreaDelivery(name, minPrice, deliveryPrice, uniqId)

        this.render()
        loader.stop()

        Dialog.close('#areaDeliveryEditDialog')
    }

    singleSaveAreaDelivery(name, minPrice, deliveryPrice, uniqId) {
        const areaDelivery = this.findAreaByUniqId(uniqId)

        if (areaDelivery) {
            areaDelivery.NameArea = name
            areaDelivery.MinPrice = minPrice
            areaDelivery.DeliveryPrice = deliveryPrice
        } else {
            const newAreaDelivery = {
                UniqId: uniqId,
                NameArea: name,
                MinPrice: minPrice,
                DeliveryPrice: deliveryPrice
            }

            AreaDelivery.push(newAreaDelivery)
        }
    }

    multiSaveAreaDelivery(names, minPrice, deliveryPrice) {
        for (name of names) {
            const uniqId = generateRandomString(10)
            const areaDelivery = this.findAreaByName(names)

            if (areaDelivery) {
                areaDelivery.NameArea = name
                areaDelivery.MinPrice = minPrice
                areaDelivery.DeliveryPrice = deliveryPrice
            } else {
                const newAreaDelivery = {
                    UniqId: uniqId,
                    NameArea: name,
                    MinPrice: minPrice,
                    DeliveryPrice: deliveryPrice
                }

                AreaDelivery.push(newAreaDelivery)
            }
        }
    }

    findAreaByUniqId(uniqId) {
        for (let areaDelivery of AreaDelivery) {
            if (areaDelivery.UniqId === uniqId) {
                return areaDelivery
            }
        }

        return null
    }

    findAreaByName(name) {
        for (let areaDelivery of AreaDelivery) {
            if (areaDelivery.NameArea === name) {
                return areaDelivery
            }
        }

        return null
    }
}

function getProductType($items) {
    const productTypes = []
    let productType = ProductType.Normal

    $items.each(function () {
        productTypes.push(parseInt($(this).attr('value')))
    })

    for (type of productTypes) {
        productType = BitOperation.Add(productType, type)
    }

    return productType
}

function setProductType(productType) {
    const $selectProductType = $('#product-type')
    $selectProductType[0].sumo.unSelectAll()
    $selectProductType[0].sumo.reload()

    for (key in ProductType) {
        const type = ProductType[key]
        if (BitOperation.isHas(productType, type))
            $selectProductType[0].sumo.selectItem(type.toString())
    }

}


function onChangeDeliveryMaxDate(e, defaultValue = 0) {
    //format hh:mm
    const reg = '^[ 0-9]+$'
    let value = $(e).val()
    const processingValue = value.match(reg)

    if (!processingValue)
        $(e).val(defaultValue)
}

function openSttingOnlinePay() {
    const setting = new OnlinePaySetting()
    setting.render()

    Dialog.showModal('#onlinePayDeliverySettingDialog')
}

class OnlinePaySetting {
    render() {
        const $paymentCbx = $('#payment-online')
        $paymentCbx.prop("checked", OnlinePayData.isPayOnline)
        $('#payment-condition').prop("checked", OnlinePayData.isAcceptedOnlinePayCondition)
        $('#merchant-id').val(OnlinePayData.merchantId)
        $('#payment-key').val(OnlinePayData.paymentKey)
        $('#credit-key').val(OnlinePayData.creditKey)

        $paymentCbx.unbind('change')
        $paymentCbx.bind('change', this.pymentOnlineToggle)

        this.toggleInputs()
    }

    pymentOnlineToggle = () => {
        this.toggleInputs()
    }

    toggleInputs() {
        const $dialog = $('#onlinePayDeliverySettingDialog')
        const $inputs = $dialog.find('.group input')
        const isPayOnline = $('#payment-online').is(':checked')

        $inputs.attr('disabled', !isPayOnline)

        let $payCondition = $dialog.find('#payment-condition')
        if (!isPayOnline) {
            $inputs.val('')
            $payCondition.prop('checked', false)
            $payCondition.attr('disabled', true)
        } else
            $payCondition.attr('disabled', false)
    }

    static done() {
        const isPayOnline = $('#payment-online').is(':checked')
        const isAccepted = $('#payment-condition').is(':checked')
        const merchantId = $('#merchant-id').val()
        const paymentKey = $('#payment-key').val()
        const creditKey = $('#credit-key').val()

        if (isPayOnline && (
            !isAccepted ||
            !merchantId ||
            !paymentKey ||
            !creditKey)) {

            showErrorMessage('Заполните все поля')

            return
        }

        OnlinePayData.isPayOnline = isPayOnline
        OnlinePayData.merchantId = merchantId
        OnlinePayData.paymentKey = paymentKey
        OnlinePayData.creditKey = creditKey
        OnlinePayData.isAcceptedOnlinePayCondition = isAccepted

        Dialog.close('#onlinePayDeliverySettingDialog')
    }
}