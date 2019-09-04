$(document).ready(function () {
    bindSelectSumo();
    initHistoryOrderDatePicker();
    bindDialogCloseClickBackdor();

    let bindShowModal = function (id, dialogId, additionalFunc, predicate) {
        $(`#${id}`).bind("click", function () {
            let addCategoryDialog = $(`#${dialogId}`);

            if (!predicate || predicate()) {
                Dialog.showModal(addCategoryDialog);
            }

            if (additionalFunc) {
                additionalFunc();
            }
        });
    }

    $("#setting-phone-number,#setting-phone-number-additional").mask("+7(999)999-99-99");

    $(".menu-item").not(".logout").bind("click", function () {
        selectMenuItem(this);
    });

    let setOperationAdd = () => CurrentOperation = TypeOperation.Add;
    let productPredicate = () => {
        if (!SelectIdCategoryId) {
            showInfoMessage("Выберите категорию");
            return false;
        }

        return true;
    };

    let stockOkFunc = () => {
        stockTypeToggleDiscountn();
        setOperationAdd();
    };

    bindShowModal("add-category", "addCategoryDialog", setOperationAdd);
    bindShowModal("add-product", "addProducDialog", setOperationAdd, productPredicate);
    bindShowModal("add-branch", "addBranchDialog", addNewBranchLoginData);
    bindShowModal("add-stock", "addStockDialog", stockOkFunc);

    $("input[type=file]").change(function () {
        addPreviewImage(this);
    });

    $("#stock-type").bind("change", stockTypeToggleDiscountn);
    bindDragula();

    if ($(".header-menu .menu-item-active").attr("target-id") == Pages.Order) {
        loadOrders();
    }
});

function bindDialogCloseClickBackdor() {
    $("dialog").bind('click', function (event) {
        var rect = this.getBoundingClientRect();
        var isInDialog = false;

        if (typeof (event.clientY) === typeof (undefined)) {
            isInDialog = true;
        }
        else {
            isInDialog = (rect.top <= event.clientY && event.clientY <= rect.top + rect.height
                && rect.left <= event.clientX && event.clientX <= rect.left + rect.width);
        }

        if (!isInDialog) {
            Dialog.close($(this));
        }
    });
}

const Pages = {
    Order: 'order',
    HistoryOrder: 'history',
    Products: 'products',
    Stock: 'stock',
    Branch: 'branch',
    Settong: 'setting',
    Delivery: 'delivery',
    Analytics: 'analytics'
}

var OrderHistoryDatePicker;
function initHistoryOrderDatePicker() {
    var prevDate = new Date();
    prevDate.setDate(prevDate.getDate() - 30);

    let options = {
        position: "bottom center",
        range: true,
        multipleDatesSeparator: " - ",
        toggleSelected: false,
        onHide: function (dp, animationCompleted) {
            if (!dp.maxRange && !animationCompleted) {
                dp.selectDate(dp.minRange);
            }

            if (!animationCompleted) {
                loadHistoryOrders();
            }
        }
    };
    let $inputDate = $("#order-history-period");
    $inputDate.datepicker(options);
    OrderHistoryDatePicker = $inputDate.data("datepicker");

    $inputDate.next("i").bind("click", function () {
        OrderHistoryDatePicker.show();
    })

    OrderHistoryDatePicker.selectDate([prevDate, new Date()]);
}

var AdditionalBranch = [];
var AdditionalHistoryBranch = [];
function bindSelectSumo() {
    $("#show-additional-order,#show-additional-history-order").SumoSelect({
        okCancelInMulti: true,
        placeholder: 'Заказы из других городов'
    });

    let updateListenBranchIds = additionalBrunchIds => {
        let currentBrunchId = getCurrentBranchId();
        let listenNewBranchIds = [...additionalBrunchIds, currentBrunchId];

        addListenByBranch(listenNewBranchIds);
    };

    $(`#additional-order-city .btnOk`).bind("click", function () {
        AdditionalBranch = [];
        $('#show-additional-order option:selected').each(function (i) {
            AdditionalBranch.push($(this).attr("key"));
        });

        updateListenBranchIds(AdditionalBranch);
        loadOrders(true);
    });

    $(`#additional-history-order-city .btnOk`).bind("click", function () {
        AdditionalHistoryBranch = [];
        $('#additional-history-order-city option:selected').each(function (i) {
            AdditionalHistoryBranch.push($(this).attr("key"));
        });

        updateListenBranchIds(AdditionalHistoryBranch);
        loadHistoryOrders();
    });
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
        calcOrderNumbers(TypeItem.Categories);
    });
    dragula([document.getElementById("product-list")], {
        revertOnSpill: true
    }).on("drop", function () {
        calcOrderNumbers(TypeItem.Products);
    });;
}

function calcOrderNumbers(typeItem) {
    let $items = [];
    let updaterOrderNumber = []
    let attrName = "";
    let url = "";

    switch (typeItem) {
        case TypeItem.Categories:
            $items = $("#category-list .category-item");
            attrName = "category-id";
            url = "/Admin/UpdateOrderNumberCategory";
            break;
        case TypeItem.Products:
            $items = $("#product-list .product-item");
            attrName = "product-id";
            url = "/Admin/UpdateOrderNumberProducts";
            break;
    }

    if ($items.length > 0) {
        for (let i = 0; i < $items.length; ++i) {
            let id = $($items[i]).attr(attrName);

            updaterOrderNumber.push({
                Id: id,
                OrderNumber: i + 1,
            });
        }

        $.post(url, { data: updaterOrderNumber }, null);
    }
}

var DataProduct = {
    Categories: [],
    Products: []
}

var StockType = {
    Custom: 0,
    FirstOrder: 1,
    OrderTakeYourself: 2
}

function stockTypeToggleDiscountn() {
    let currentType = parseInt($("#stock-type").val());

    if (currentType == StockType.Custom) {
        $("#stock-discount").attr("disabled", true);
    } else {
        $("#stock-discount").removeAttr("disabled");
    }
}

function initBlocks() {
    $('#stock-list').BlocksIt({
        numOfCol: 2,
        boxClass: '.stock-item',
        offsetX: 5,
        offsetY: 5
    });
}

var TypeOperation = {
    Add: 0,
    Update: 1
}
var CurrentOperation;

function addNewBranchLoginData() {
    let login = generateRandomString();
    let password = generateRandomString();

    $("#login-new-branch").val(login);
    $("#password-new-branch").val(password);
}

function logout() {
    let successFunc = (result) => {
        window.location.href = result.URL;
    }

    $.post("/Home/Logout", null, successCallBack(successFunc, null));
}

function prevChangedPage(page) {
    switch (page) {
        case Pages.Order:
            resetSearchForOrderNumber(page);
            break;
        case Pages.HistoryOrder:
            resetSearchForOrderNumber(page);
            break
    }
}

function postChangedPage(page) {
    switch (page) {
        case Pages.Stock:
            loadStockList();
            break
        case Pages.Analytics: {
            loadAnalyticsReport();
        }
    }
}

function loadAnalyticsReport() {
    const containerId = "analytics-wrapper";
    const currentBrunchId = getCurrentBranchId();

    $(`#${containerId}`).empty();

    new CountOrderReport(containerId, currentBrunchId, URLAnalytics);
    new RevenueReport(containerId, currentBrunchId, URLAnalytics)
    new Top5Categories(containerId, currentBrunchId, URLAnalytics);
    new Top5Products(containerId, currentBrunchId, URLAnalytics);
    new DeliveryMethod(containerId, currentBrunchId, URLAnalytics);
    new NewUsersReport(containerId, currentBrunchId, URLAnalytics);
}

function resetSearchForOrderNumber(containerId) {
    clearSearchInput(containerId);
    searchByOrderNumber(containerId, false);
}

function clearSearchInput(containerId) {
    $(`#${containerId} .search-input input`).val("");
}

function selectMenuItem(e) {
    let $e = $(e);
    let targetId = $e.attr("target-id");

    if ($e.hasClass("menu-item-active")) {
        return;
    }

    prevChangedPage(targetId);

    $(".menu-item").removeClass("menu-item-active");
    $e.addClass("menu-item-active");
    $(".section").addClass("hide");
    $(`#${targetId}`).removeClass("hide");

    postChangedPage(targetId);
}

function cancelDialog(e) {
    let dialog = $(e);

    dialog.find("input").val("");
    dialog.find("textarea").val("");
    dialog.find(".dialog-image-upload").removeClass("hide");
    dialog.find("img").addClass("hide");
    dialog.find("option").removeAttr("selected");
    dialog.find("select").val("0")
    Dialog.close(dialog);
}

function operationCategory() {
    switch (CurrentOperation) {
        case TypeOperation.Add:
            addCategory();
            break;
        case TypeOperation.Update:
            updateCategory();
            break;
    }
}

function operationProduct() {
    if (!SelectIdCategoryId) {
        showInfoMessage("Выберите категорию")
    }

    switch (CurrentOperation) {
        case TypeOperation.Add:
            addProduct();
            break;
        case TypeOperation.Update:
            updateProduct();
            break;
    }
}

function operationStock() {

    switch (CurrentOperation) {
        case TypeOperation.Add:
            addStock();
            break;
        case TypeOperation.Update:
            updateStock();
            break;
    }
}

function getStockById(searchId) {

    for (let id in StockList) {
        if (StockList[id].Id == searchId) {
            return StockList[id];
        }
    }
}

function getIndexStockById(searchId) {
    for (let id in StockList) {
        if (StockList[id].Id == searchId) {
            return id;
        }
    }
}

function removeStock(id) {
    $(`[stock-id=${id}]`).fadeOut(1000, function () {
        $(this).remove();
        initBlocks();

        var stockIndex = getIndexStockById(id);
        StockList.splice(stockIndex, 1);
    });

    $.post("/Admin/RemoveStock", { id: id }, null);
}

function editStock(id) {
    CurrentOperation = TypeOperation.Update;

    let dialog = $("#addStockDialog");
    let stock = getStockById(id);

    dialog.find("#stock-id").val(stock.Id);
    dialog.find("#name-stock").val(stock.Name);
    dialog.find("#stock-discount").val(stock.Discount);
    dialog.find("#description-stock").val(stock.Description);
    dialog.find("img").attr("src", stock.Image);

    let $selectStockype = dialog.find("#stock-type");
    $selectStockype.find("option").removeAttr("selected");
    $selectStockype.find(`[value=${stock.StockType}]`).attr("selected", true);


    if (stock.Image.indexOf("default") == -1) {
        dialog.find("img").removeClass("hide");
        dialog.find(".dialog-image-upload").addClass("hide");
    }

    //dialog.trigger("showModal");
    Dialog.showModal(dialog);
}

function isVisibleStock(id) {
    let stock = getStockById(id);

    return stock.Visible;
}

function updateRenderStock(container, stock) {
    let $stock = $(container);

    $stock.find("img").attr("src", stock.Image);
    $stock.find(".stock-item-name").html(stock.Name);
    toggleVisibleSotck(container, stock);
}

function updateStock() {
    let files = $("#addStockDialog input[type=file]")[0].files;
    var dataImage = new FormData();

    for (var x = 0; x < files.length; x++) {
        dataImage.append("file" + x, files[x]);
    }

    let functUpdate = function (data) {
        let stockId = $("#stock-id").val();
        let stock = {
            Id: stockId,
            Name: $("#name-stock").val(),
            Discount: $("#stock-discount").val(),
            Description: $("#description-stock").val(),
            Image: data.URL,
            Visible: isVisibleStock(stockId),
            StockType: parseInt($("#stock-type option:selected").attr("value"))
        }

        let loader = new Loader($("#addStockDialog form"));
        let successFunc = function (result, loader) {
            loader.stop();
            if (result.Success) {
                let index = getIndexStockById(result.Data.Id);
                StockList[index] = result.Data;

                let $stock = $(`[stock-id=${result.Data.Id}]`);

                updateRenderStock($stock, result.Data);
                cancelDialog("#addStockDialog");
            } else {
                showErrorMessage(result.ErrorMessage);
            }
        }
        loader.start();

        $.post("/Admin/UpdateStock", stock, successCallBack(successFunc, loader));
    }



    if (files.length == 0) {
        let data = {
            URL: $("#addStockDialog img").attr("src")
        }

        functUpdate(data);
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/Admin/UploadImage',
        contentType: false,
        processData: false,
        data: dataImage,
        success: function (data) {
            functUpdate(data);
        }
    });
}

function updateVisibleStock(stock) {
    $.post("/Admin/UpdateStock", stock, null);
}

function addStock() {
    let loader = new Loader($("#addStockDialog  form"));
    loader.start();

    let files = $("#addStockDialog input[type=file]")[0].files;
    var dataImage = new FormData();

    for (var x = 0; x < files.length; x++) {
        dataImage.append("file" + x, files[x]);
    }

    let addFunc = function (data) {
        let stock = {
            Name: $("#name-stock").val(),
            Discount: $("#stock-discount").val(),
            Description: $("#description-stock").val(),
            Image: data.URL,
            Visible: true,
            StockType: parseInt($("#stock-type option:selected").attr("value"))
        }
        let successFunc = function (result, loader) {
            loader.stop();
            if (result.Success) {
                $("#stock .empty-list").remove();
                StockList.push(result.Data);
                addStockToList(result.Data);
                cancelDialog("#addStockDialog");
            } else {
                showErrorMessage(result.ErrorMessage);
            }
        }

        $.post("/Admin/AddStock", stock, successCallBack(successFunc, loader));
    }

    if (files.length == 0) {
        let data = {
            URL: $("#addStockDialog img").attr("src")
        }

        addFunc(data);
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/Admin/UploadImage',
        contentType: false,
        processData: false,
        data: dataImage,
        success: function (data) {
            addFunc(data);
        }
    });
}

var SelectIdCategoryId;

function updateCategory() {
    let loader = new Loader($("#addCategoryDialog form"));
    loader.start();

    let files = $("#addCategoryDialog input[type=file]")[0].files;
    var dataImage = new FormData();

    for (var x = 0; x < files.length; x++) {
        dataImage.append("file" + x, files[x]);
    }

    let uppFunc = (data) => {
        let category = {
            Id: $("#category-id").val(),
            Name: $("#name-category").val(),
            Image: data.URL
        }

        let successFunc = function (result, loader) {
            loader.stop();
            if (result.Success) {
                let categoryItem = $(`[category-id=${category.Id}]`);
                categoryItem.find(".category-item-image img").attr("src", category.Image);
                categoryItem.find(".category-item-name").html(category.Name);

                cancelDialog("#addCategoryDialog");
            } else {
                showErrorMessage(result.ErrorMessage);
            }
        }

        $.post("/Admin/UpdateCategory", category, successCallBack(successFunc, loader));
    }

    if (files.length == 0) {
        let data = {
            URL: $("#addCategoryDialog img").attr("src")
        }

        addFunc(data);

        return;
    }

    $.ajax({
        type: 'POST',
        url: '/Admin/UploadImage',
        contentType: false,
        processData: false,
        data: dataImage,
        success: function (data) {
            uppFunc(data);
        }
    });
}

function addCategory() {
    let loader = new Loader($("#addCategoryDialog form"));
    loader.start();

    let files = $("#addCategoryDialog input[type=file]")[0].files;
    var dataImage = new FormData();

    for (var x = 0; x < files.length; x++) {
        dataImage.append("file" + x, files[x]);
    }

    let addFunc = function (data) {
        let category = {
            Name: $("#name-category").val(),
            Image: data.URL
        }

        let successFunc = function (result, loader) {
            loader.stop();
            if (result.Success) {
                DataProduct.Categories.push(result.Data);
                $(".category .empty-list").remove();
                addCategoryToList(result.Data);
                cancelDialog("#addCategoryDialog");
            } else {
                showErrorMessage(result.ErrorMessage);
            }
        }

        $.post("/Admin/AddCategory", category, successCallBack(successFunc, loader));
    }

    if (files.length == 0) {
        let data = {
            URL: $("#addCategoryDialog img").attr("src")
        }

        addFunc(data);

        return;
    }

    $.ajax({
        type: 'POST',
        url: '/Admin/UploadImage',
        contentType: false,
        processData: false,
        data: dataImage,
        success: function (data) {
            addFunc(data);
        }
    });
}

function addCategoryToList(category) {
    let templateCategoryItem = `
    <div class="category-item" onclick="selectCategory(this)" category-id="${category.Id}">
        <div class="category-item-image">
            <img src="${category.Image}" />
        </div>
        <div class="category-item-name">
            ${category.Name}
        </div>
        <div class="category-item-action">
            <i onclick="editCategory(this, event);" class="fal fa-edit"></i>
            <i class="fal fa-eye item-show ${(category.Visible ? '' : 'hide')}" onclick="toggleShowItem(this, ${TypeItem.Categories}, event);"></i>
            <i class="fal fa-eye-slash item-hide ${(category.Visible ? 'hide' : '')}" onclick="toggleShowItem(this, ${TypeItem.Categories}, event);"></i>
            <i onclick="removeCategory(this, event);" class="fal fa-trash-alt"></i>
        </div>
    </div >`;

    $(".category-list").append(templateCategoryItem);
}

function editCategory(e, event) {
    event.stopPropagation();

    CurrentOperation = TypeOperation.Update;

    let dialog = $("#addCategoryDialog");
    let parent = $($(e).parents(".category-item"));

    let category = {
        Id: parent.attr("category-id"),
        Name: parent.find(".category-item-name").html().trim(),
        Image: parent.find("img").attr("src")
    }

    dialog.find("#category-id").val(category.Id);
    dialog.find("#name-category").val(category.Name);
    dialog.find("img").attr("src", category.Image);

    if (category.Image.indexOf("default") == -1) {
        dialog.find("img").removeClass("hide");
        dialog.find(".dialog-image-upload").addClass("hide");
    }

    //dialog.trigger("showModal");
    Dialog.showModal(dialog);
}

function removeCategory(e, event) {
    event.stopPropagation();

    let callback = function () {
        let parent = $($(e).parents(".category-item"));
        let id = parent.attr("category-id");
        let loader = new Loader(parent);
        let successFunc = function (result, loader) {
            loader.stop();
            if (result.Success) {
                if (SelectIdCategoryId == id) {
                    SelectIdCategoryId = null;

                    clearProductList();
                    setEmptyProductInfo();
                }

                $(`[category-id=${id}]`).fadeOut(500, function () {
                    $(this).remove();

                    if ($(".category-list").children().length == 0) {
                        setEmptyCategoryInfo();
                    }
                });

            } else {
                showErrorMessage(result.ErrorMessage);
            }
        }
        loader.start();

        $.post("/Admin/RemoveCategory", { id: id }, successCallBack(successFunc, loader));
    }


    deleteConfirmation(callback)
}

function selectCategory(e) {
    $(".category-list .select-category").removeClass("select-category");
    $(e).addClass("select-category");
    categoryId = $(e).attr("category-id");

    if (categoryId == SelectIdCategoryId) {
        return;
    }

    SelectIdCategoryId = categoryId;
    loadProductList(SelectIdCategoryId);
}

function sortByOrderNumber(data) {
    var newData = [];

    for (var item of data) {
        newData[item.OrderNumber - 1] = item;
    }

    return newData;
}

function loadCategoryList() {
    SelectIdCategoryId = null;
    clearCategoryList();

    DataProduct.Products = [];
    let container = $(".category-list");
    let loader = new Loader($(".category"));
    let successFunc = function (result, loader) {
        loader.stop();
        if (result.Success) {
            if (!result.Data || result.Data.length == 0) {
                setEmptyCategoryInfo();
            } else {
                DataProduct.Categories = sortByOrderNumber(result.Data);

                for (let category of DataProduct.Categories) {
                    addCategoryToList(category);
                }

            }

        } else {
            showErrorMessage(result.ErrorMessage);
            setEmptyCategoryInfo();
        }
    }
    loader.start();

    $.post("/Admin/LoadCategoryList", null, successCallBack(successFunc, loader));
}

function loadProducts() {
    clearCategoryList();
    clearProductList();
    setEmptyProductInfo();

    loadCategoryList();
}

function addProduct() {
    let loader = new Loader($("#addProducDialog  form"));
    loader.start();

    let files = $("#addProducDialog input[type=file]")[0].files;
    var dataImage = new FormData();

    for (var x = 0; x < files.length; x++) {
        dataImage.append("file" + x, files[x]);
    }

    let addFunc = function (data) {
        let product = {
            CategoryId: SelectIdCategoryId,
            Name: $("#name-product").val(),
            AdditionInfo: $("#product-additional-info").val(),
            Price: $("#product-price").val(),
            Description: $("#description-product").val(),
            Image: data.URL,
            ProductType: parseInt($("#product-type option:selected").attr("value"))
        }
        let successFunc = function (result, loader) {
            loader.stop();
            if (result.Success) {
                $(".product .empty-list").remove();
                DataProduct.Products.push(result.Data);
                addProductToList(result.Data);
                cancelDialog("#addProducDialog");
            } else {
                showErrorMessage(result.ErrorMessage);
            }
        }

        $.post("/Admin/AddProduct", product, successCallBack(successFunc, loader));
    }

    if (files.length == 0) {
        let data = {
            URL: $("#addProducDialog img").attr("src")
        }

        addFunc(data);
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/Admin/UploadImage',
        contentType: false,
        processData: false,
        data: dataImage,
        success: function (data) {
            addFunc(data);
        }
    });
}

function updateProduct() {
    let loader = new Loader($("#addProducDialog form"));
    loader.start();

    let files = $("#addProducDialog input[type=file]")[0].files;
    var dataImage = new FormData();

    for (var x = 0; x < files.length; x++) {
        dataImage.append("file" + x, files[x]);
    }

    let uppProduct = (data) => {
        let product = {
            Id: $("#product-id").val(),
            CategoryId: SelectIdCategoryId,
            Name: $("#name-product").val(),
            AdditionInfo: $("#product-additional-info").val(),
            Price: $("#product-price").val(),
            Description: $("#description-product").val(),
            Image: data.URL,
            ProductType: parseInt($("#product-type option:selected").attr("value"))
        }

        let successFunc = function (result, loader) {
            loader.stop();
            if (result.Success) {
                var oldProduct = getProductById(product.Id);
                oldProduct.Image = product.Image;
                oldProduct.Name = product.Name;
                oldProduct.AdditionInfo = product.AdditionInfo;
                oldProduct.Price = product.Price;
                oldProduct.Description = product.Description;
                oldProduct.ProductType = product.ProductType;

                let productItem = $(`[product-id=${product.Id}]`);
                productItem.find(".product-item-image img").attr("src", product.Image);
                productItem.find(".product-item-name").html(product.Name);
                productItem.find(".product-item-additional-info").html(product.AdditionInfo);
                productItem.find(".product-item-price span").html(product.Price);
                productItem.find(".product-item-description").html(product.Description);
                productItem.find(".product-type-item").html(product.ProductType);


                cancelDialog("#addProducDialog");
            } else {
                showErrorMessage(result.ErrorMessage);
            }
        }

        $.post("/Admin/UpdateProduct", product, successCallBack(successFunc, loader));
    }

    if (files.length == 0) {
        let data = {
            URL: $("#addProducDialog img").attr("src")
        }

        uppProduct(data);
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/Admin/UploadImage',
        contentType: false,
        processData: false,
        data: dataImage,
        success: function (data) {
            uppProduct(data);
        }
    });
}

function addStockToList(stock) {
    let $template = $($("#stock-item-template").html());

    $template.find("#stock-id").val(stock.Id);
    $template.attr("stock-id", stock.Id);
    $template.find("img").attr("src", stock.Image);
    $template.find(".stock-item-name").html(stock.Name);
    $template.find(".stock-remove").bind("click", function () {
        let callback = () => removeStock(stock.Id);

        deleteConfirmation(callback);
    });
    $template.find(".stock-edit").bind("click", function () {
        editStock(stock.Id);
    });
    $template.find(".stock-visible").bind("click", function () {
        let changeStock = getStockById(stock.Id);

        changeStock.Visible = !changeStock.Visible;
        toggleVisibleSotck($template, changeStock);
        updateVisibleStock(changeStock);
    });
    toggleVisibleSotck($template, stock);

    $("#stock-list").append($template);
    $template.find("img").one("load", function () {
        initBlocks();
    }).each(function () {
        if (this.complete) {
            $(this).trigger('load');
        }
    });
}

function toggleVisibleSotck(container, stock) {
    let $container = $(container);

    if (stock.Visible) {
        $container.find(".stock-visible").html('<i class="fal fa-eye"></i>');
    } else {
        $container.find(".stock-visible").html('<i class="fal fa-eye-slash"></i>');
    }

}

function toggleShowItem(e, typeItem, event) {
    if (event) {
        event.stopPropagation();
    }

    let url = "";
    let $e = $(e);
    let $parent;
    let id;
    let visible;
    switch (typeItem) {
        case TypeItem.Categories:
            $parent = $e.parents(".category-item");
            id = $parent.attr("category-id");
            url = "/Admin/UpdateVisibleCategory";
            break;
        case TypeItem.Products:
            $parent = $e.parents(".product-item");
            id = $parent.attr("product-id");
            url = "/Admin/UpdateVisibleProduct";
            break;
        case TypeItem.Review:
            $parent = $e.parents(".review-item");
            id = $parent.attr("review-id");
            url = "/Admin/UpdateVisibleReview";
            break;
    }

    $e.addClass("hide");

    if ($e.hasClass("item-show")) {
        visible = false;
        $parent.find(".item-hide").removeClass("hide");
    } else {
        visible = true;
        $parent.find(".item-show").removeClass("hide");
    }

    let updaterVisible = {
        Id: id,
        Visible: visible

    };
    $.post(url, { data: updaterVisible }, null);

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
                ${product.AdditionInfo}
            </div>
            <div class="product-item-price">
                <span>${product.Price}</span>
                <i class="fal fa-ruble-sign"></i>
            </div>
            <div class="product-item-action">
                <i onclick="editProduct(this, event);" class="fal fa-edit"></i>
                <i class="fal  fa-comment-dots" onclick="openProductUserCallback(this, event)"></i>
                <i class="fal fa-eye item-show ${(product.Visible ? '' : 'hide')}" onclick="toggleShowItem(this, ${TypeItem.Products}, event);"></i>
                <i class="fal fa-eye-slash item-hide ${(product.Visible ? 'hide' : '')}" onclick="toggleShowItem(this, ${TypeItem.Products}, event);"></i>
                <i onclick="removeProduct(this, event);" class="fal fa-trash-alt"></i>
            </div>
        </div>
        <div class="product-item-description hide">
                ${product.Description}
            </div>
        <div class="product-type-item hide">
            ${product.ProductType}
        </div>
    </div >`;

    let $template = $(templateCategoryItem);

    $template.find(".product-item-raty").raty({
        score: product.Rating,
        showHalf: true,
        path: "../images/rating",
        targetKeep: true,
        precision: true,
        readOnly: true
    });

    $(".product-list").append($template);
}

function clearStockList() {
    $("#stock .empty-list").remove();
    $("#stock-list").empty();
}

function clearProductList() {
    $(".product-list").empty();
}

function clearCategoryList() {
    $(".category-list").empty();
}

function editProduct(e, event) {
    event.stopPropagation();

    CurrentOperation = TypeOperation.Update;

    let dialog = $("#addProducDialog");
    let parent = $($(e).parents(".product-item"));

    let product = {
        Id: parent.attr("product-id"),
        Name: parent.find(".product-item-name").html().trim(),
        AdditionInfo: parent.find(".product-item-additional-info").html().trim(),
        Price: parent.find(".product-item-price span").html().trim(),
        Description: parent.find(".product-item-description").html().trim(),
        Image: parent.find("img").attr("src"),
        ProductType: parseInt(parent.find(".product-type-item").html().trim())
    }

    dialog.find("#product-id").val(product.Id);
    dialog.find("#name-product").val(product.Name);
    dialog.find("#product-additional-info").val(product.AdditionInfo);
    dialog.find("#product-price").val(product.Price);
    dialog.find("#description-product").val(product.Description);
    dialog.find("img").attr("src", product.Image);

    let $selectProductType = dialog.find("#product-type");
    $selectProductType.find("option").removeAttr("selected");
    $selectProductType.find(`[value=${product.ProductType}]`).attr("selected", true);


    if (product.Image.indexOf("default") == -1) {
        dialog.find("img").removeClass("hide");
        dialog.find(".dialog-image-upload").addClass("hide");
    }

    //dialog.trigger("showModal");
    Dialog.showModal(dialog);
}

function deleteConfirmation(callback) {
    let $dialog = $("#deleteConfirmationDialog");
    let clickFunc = function () {
        if (callback) {
            callback();
        }

        cancelDialog($dialog);
    };

    $dialog.find(".btn-submit").unbind("click");
    $dialog.find(".btn-submit").bind("click", clickFunc);

    Dialog.showModal($dialog);
}

function removeProduct(e, event) {
    event.stopPropagation();

    let callback = function () {
        let parent = $($(e).parents(".product-item"));
        let id = parent.attr("product-id");
        let loader = new Loader(parent);
        let successFunc = function (result, loader) {
            loader.stop();
            if (result.Success) {
                $(`[product-id=${id}]`).fadeOut(500, function () {
                    $(this).remove();

                    if ($(".product-list").children().length == 0) {
                        setEmptyProductInfo();
                    }
                });

            } else {
                showErrorMessage(result.ErrorMessage);
            }
        }
        loader.start();

        $.post("/Admin/RemoveProduct", { id: id }, successCallBack(successFunc, loader));
    }

    deleteConfirmation(callback);
}

var StockList = [];

function addAllItemStock(data) {
    for (let stock of data) {
        addStockToList(stock);
    }
}

function loadStockList() {
    clearStockList();
    if (StockList &&
        StockList.length > 0) {
        addAllItemStock(StockList);
    } else {
        let loader = new Loader($("#stock .content-wrapper"));
        let successFunc = function (result, loader) {
            loader.stop();
            if (result.Success) {
                if (!result.Data || result.Data.length == 0) {
                    setEmptyStockInfo();
                } else {
                    StockList = result.Data;
                    addAllItemStock(StockList);
                }
            } else {
                showErrorMessage(result.ErrorMessage);
                setEmptyStockInfo();
            }
        }
        loader.start();

        $.post("/Admin/LoadStockList", null, successCallBack(successFunc, loader));
    }
}

function getProductById(id) {
    let serchProduct = null;

    for (let product of DataProduct.Products) {
        if (product.Id == id) {
            serchProduct = product;
            break;
        }
    }

    return serchProduct;
}

function loadProductList(idCategory) {
    clearProductList();
    let container = $(".product-list");
    let loader = new Loader($(".product"));
    let successFunc = function (result, loader) {
        loader.stop();
        if (result.Success) {
            if (!result.Data || result.Data.length == 0) {
                setEmptyProductInfo();
            } else {
                DataProduct.Products = sortByOrderNumber(result.Data);
                for (let product of DataProduct.Products) {
                    addProductToList(product);
                }


            }
        } else {
            showErrorMessage(result.ErrorMessage);
            setEmptyProductInfo();
        }
    }
    loader.start();

    $.post("/Admin/LoadProductList", { idCategory: idCategory }, successCallBack(successFunc, loader));
}

function addPreviewImage(input) {
    if (input.files && input.files[0]) {
        let reader = new FileReader();

        reader.onload = function (e) {
            let dialog = $(input).parents("dialog");
            let img = dialog.find("img");

            dialog.find(".dialog-image-upload").addClass("hide");
            img.attr("src", e.target.result);
            img.removeClass("hide");
        }

        reader.readAsDataURL(input.files[0]);
    }
}

function openDialogFile(id) {
    $(`#${id}`).click();
}

function saveSetting() {
    let warnMsg = {
        City: "Выберите город из списка",
        Street: "Укажите имя улицы",
        HomeNumber: "Укажите номер дома",
        PhoneNumber: "Укажите номер телефона"
    }

    let cityId = $("#setting-city-list option[value='" + $('#setting-city').val() + "']").attr('city-id');
    let street = $("#setting-street").val();
    let homeNumber = $("#setting-home").val();
    let phoneNumber = $("#setting-phone-number").val();

    if (!cityId) {
        showWarningMessage(warnMsg.City);
        return;
    }
    if (!street) {
        showWarningMessage(warnMsg.Street);
        return;
    }
    if (!homeNumber) {
        showWarningMessage(warnMsg.HomeNumber);
        return;
    }
    if (!phoneNumber) {
        showWarningMessage(warnMsg.PhoneNumber);
        return;
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
    let loader = new Loader($("#setting"));
    let successFunc = function (result, loader) {
        loader.stop();
        if (!result.Success) {
            showErrorMessage(result.ErrorMessage);
        }
    }

    loader.start();

    $.post("/Admin/SaveSetting", setting, successCallBack(successFunc, loader));
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
    let timeDays = {};
    for (let day in DayWeekly) {
        let timeDay = $(`[day-id=${day}]`);
        let checked = timeDay.find("input[type=checkbox]").is(":checked");
        let start = parseFloat(timeDay.find("[name=start]").val()).toFixed(2);
        let end = parseFloat(timeDay.find("[name=end]").val()).toFixed(2);

        if (isNaN(start)) {
            start = "0.00";
        }

        if (isNaN(end)) {
            end = "0.00";
        }

        timeDays[DayWeekly[day]] = checked ? [start, end] : null;
    }

    return JSON.stringify(timeDays);
}

function saveDeliverySetting() {
    let warnMgs = {
        PriceDelivery: "Укажите стоимость доставки",
        FreePriceDelivery: "Укажите минимальные суммы закаов для бесплатной доставки в районы",
    }

    let priceDelivery = $("#price-delivery").val();
    let freePriceDelivery = $("#free-delivery").val();

    if (!priceDelivery) {
        showWarningMessage(warnMgs.PriceDelivery);
        return;
    }

    if (!AreaDelivery || AreaDelivery.length == 0) {
        showWarningMessage(warnMgs.FreePriceDelivery);
        return;
    }

    let setting = {
        PriceDelivery: priceDelivery,
        FreePriceDelivery: freePriceDelivery,
        IsSoundNotify: $("#sound-nodify").is(":checked"),
        ZoneId: $("#delivery-time-zone").val(),
        PayCard: $("#payment-card").is(":checked"),
        PayCash: $("#payment-cash").is(":checked"),
        TimeDeliveryJSON: getTimeDeliveryJSON(),
        AreaDeliveries: AreaDelivery
    }
    let loader = new Loader($("#delivery"));
    let successFunc = function (result, loader) {
        loader.stop();
        if (!result.Success) {
            showErrorMessage(result.ErrorMessage);
        }
    }

    loader.start();

    $.post("/Admin/SaveDeliverySetting", setting, successCallBack(successFunc, loader));
}

function addNewBranchToAdditionalOrder(brachId, cityName) {
    let option = `<option key="${brachId}">${cityName}</option>`;
    let $additionaOrder = $("#show-additional-order");
    let $additionaOrderHistory = $("#show-additional-history-order");

    $additionaOrder.append(option);
    $additionaOrder[0].sumo.reload()
    $additionaOrderHistory.append(option);
    $additionaOrderHistory[0].sumo.reload()
}

function removeBranchFromAdditionalOrder(brachId) {
    let $additionaOrder = $("#show-additional-order");
    let $additionaOrderHistory = $("#show-additional-history-order");

    $additionaOrder.find(`option[key=${brachId}]`).remove();
    $additionaOrder[0].sumo.reload()
    $additionaOrderHistory.find(`option[key=${brachId}]`).remove();
    $additionaOrderHistory[0].sumo.reload()
}

function addBranch() {
    let newBranch = {
        Login: $("#login-new-branch").val(),
        Password: $("#password-new-branch").val(),
        CityId: $("#branch-city-list option[value='" + $('#branch-city').val() + "']").attr('city-id'),
    }
    let loader = new Loader($("#addBranchDialog form"));
    let successFunc = function (result, loader) {
        loader.stop();
        if (result.Success) {
            addBranchToList(result.Data);
            //addNewBranchToAdditionalOrder(result.Data.Id, result.Data.City);
            cancelDialog("#addBranchDialog");
        } else {
            showErrorMessage(result.ErrorMessage);
        }
    }
    loader.start();

    $.post("/Admin/AddBranch", newBranch, successCallBack(successFunc, loader));
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
    </div >`;

    $(".branch-list").append(templateBranchItem);
}

function loadBranchList() {
    let container = $(".branch-list");
    container.empty();
    let loader = new Loader(container);
    let successFunc = function (result, loader) {
        loader.stop();
        if (result.Success) {
            for (let branchView of result.Data) {
                addBranchToList(branchView);
            }

        } else {
            showErrorMessage(result.ErrorMessage);
        }
    }
    loader.start();

    $.post("/Admin/LoadBranchList", null, successCallBack(successFunc, loader));
}

function removeBranch(e, id) {
    if ($(e).hasClass("disbled")) {
        return;
    }
    let parent = $($(e).parents(".branch-item"));
    let loader = new Loader(parent);
    let successFunc = function (result, loader) {
        loader.stop();
        if (result.Success) {
            $(`[branch-id=${id}]`).fadeOut(500, function () {
                $(this).remove();
            });
            removeBranchFromAdditionalOrder(id);
        } else {
            showErrorMessage(result.ErrorMessage);
        }
    }
    loader.start();

    $.post("/Admin/RemoveBranch", { id: id }, successCallBack(successFunc, loader));
}

function setEmptyProductInfo() {
    let template = `
        <div class="empty-list">
            <i class="fal fa-smile-plus"></i>
            <span>Выберите категорию и добавте продукт</span>
        </div>
    `;

    $(".product-list").html(template);
}

function setEmptyCategoryInfo() {
    let template = `
        <div class="empty-list">
            <i class="fal fa-smile-plus"></i>
            <span>Пока нет ни одной категории</span>
        </div>
    `;

    $(".category-list").html(template);
}

function setEmptyStockInfo() {
    let template = `
        <div class="empty-list">
            <i class="fal fa-smile-plus"></i>
            <span>Пока нет ни одной акции</span>
        </div>
    `;

    $("#stock .content-wrapper").append(template);
}

function setEmptyReview() {
    let template = `
        <div class="empty-list">
            <i class="fal fa-comment-edit"></i>
            <span>Пока нет ни одного отзыва</span>
        </div>
    `;

    $("#productUserCallbackDialog .product-user-callback-review-list").append(template);
}

function renderReviewItem(data) {
    let $template = $($("#review-item-template").html());
    let userName = `Клиент: ${data.PhoneNumber}`;


    $template.attr("review-id", data.Id);
    $template.find(".review-item-header span").html(userName);
    $template.find(".review-item-text").html(data.ReviewText);

    if (data.Visible) {
        $template.find(".item-show").removeClass("hide");
        $template.find(".item-hide").addClass("hide");
    } else {
        $template.find(".item-show").addClass("hide");
        $template.find(".item-hide").removeClass("hide");
    }

    return $template;
}

function cleanProductUserCallbackDialog() {
    let $dialog = $("#productUserCallbackDialog");

    $dialog.find("img").attr("src", "");
    $dialog.find(".product-name-user-callback").html("");
    $dialog.find(".product-user-callback-review-list").html("");
    $dialog.find(".product-raty-text-user-callback").html("");
}

function num2str(n, text_forms) {
    n = Math.abs(n) % 100; var n1 = n % 10;
    if (n > 10 && n < 20) { return text_forms[2]; }
    if (n1 > 1 && n1 < 5) { return text_forms[1]; }
    if (n1 == 1) { return text_forms[0]; }
    return text_forms[2];
}

function openProductUserCallback(e, event) {
    if (event) {
        event.stopPropagation();
    }

    cleanProductUserCallbackDialog();
    Reviews = [];

    let $parent = $(e).parents(".product-item");
    let loader = new Loader($parent);

    loader.start();

    let productId = $parent.attr("product-id");
    let callback = () => {
        let templateReviews = [];
        for (let review of Reviews) {
            templateReviews.push(renderReviewItem(review));
        }

        let product = getProductById(productId);
        var votesCountStr = num2str(product.VotesCount, ["голос", "голоса", "голосов"]);
        let reviewText = `Оценка: ${product.Rating.toFixed(1)} - ${product.VotesCount} ${votesCountStr}`;

        let $dialog = $("#productUserCallbackDialog");

        let $img = $dialog.find("img");
        $img.attr("src", product.Image);
        $img.removeClass("hide");
        $dialog.find(".product-name-user-callback").html(product.Name)
        $dialog.find(".product-raty-text-user-callback").html(reviewText)
        $dialog.find(".product-raty-image-user-callback").raty({
            score: product.Rating,
            showHalf: true,
            path: "../images/rating",
            targetKeep: true,
            precision: true,
            readOnly: true
        });

        $dialog.find(".product-user-callback-review-list").html(templateReviews);

        if (!Reviews || Reviews.length == 0) {
            setEmptyReview();
        }

        //$dialog.trigger("showModal");
        Dialog.showModal($dialog);

        loader.stop();
    };

    loadProductReviews(productId, callback);
}

var Reviews = [];

function loadProductReviews(productId, callback) {
    let successFunc = function (result) {
        if (result.Success) {
            if (result.Data &&
                result.Data.length > 0) {
                Reviews = result.Data;
            }
        } else {
            showErrorMessage(result.ErrorMessage);
        }

        if (callback) {
            callback();
        }
    }

    $.post("/Admin/LoadProductReviews", { productId: productId }, successCallBack(successFunc));
}

function setEmptyOrders(containerId) {
    containerId = containerId || getCurrentSectionId();
    let template = `
        <div class="empty-list">
            <div class="empty-list-animation">
            <img src="../Images/loader-empty-order.gif"/>
            </div>
            <span>Пока нет ни одного заказа</span>
        </div>
    `;

    $(`#${containerId} .order-list-grid`).append(template);
}

function removeEmptyOrders(containerId) {
    containerId = containerId || getCurrentSectionId();
    $(`#${containerId} .order-list-grid .empty-list`).remove();
}

function getCurrentBranchId() {
    return $("#current-brnach").val();
}

var Orders = [];

function loadOrders(reload = false) {
    if (Orders.length != 0 && !reload) {
        return;
    }

    clearSearchInput(Pages.Order);
    let currentBranchId = getCurrentBranchId();
    let brnachIds = [...AdditionalBranch];
    brnachIds.push(currentBranchId);

    let $list = $("#order .orders .order-list");
    $list.empty();

    let loader = new Loader($("#order"));
    loader.start();

    let successFunc = function (data, loader) {
        if (data.Success) {
            Orders = processingOrders(data.Data.Orders);
            showCountOrder(Orders.length);
            setTodayDataOrders(data.Data.TodayData, Pages.Order);

            if (Orders.length == 0) {
                setEmptyOrders(Pages.Order);
            } else {
                CardOrderRenderer.renderOrders(Orders, Pages.Order, 600);
            }
        } else {
            showErrorMessage(data.ErrorMessage);
        }

        loader.stop();
    }

    $.post("/Admin/LoadOrders", { brnachIds: brnachIds }, successCallBack(successFunc, loader));
}

var HistoryOrders = [];

function clearOrdersContainer(containerId) {
    $(`#${containerId} .order-list-grid`).empty();
}

function loadHistoryOrders() {
    clearSearchInput(Pages.HistoryOrder);
    currentBranchId = getCurrentBranchId();
    let brnachIds = [...AdditionalHistoryBranch];

    brnachIds.push(currentBranchId);

    let $list = $("#history .orders .order-list");
    $list.empty();

    let loader = new Loader($("#history"));
    loader.start();

    let successFunc = function (data, loader) {
        if (data.Success) {
            HistoryOrders = processingOrders(data.Data);
            clearOrdersContainer(Pages.HistoryOrder);

            if (HistoryOrders.length == 0) {
                setEmptyOrders(Pages.HistoryOrder);
            } else {
                removeEmptyOrders(Pages.HistoryOrder);
                CardOrderRenderer.renderOrders(HistoryOrders, Pages.HistoryOrder, 600);
            }

            changeHistoryOrderDataForStatusBar();
        } else {
            showErrorMessage(data.ErrorMessage);
        }

        loader.stop();
    }

    $.post("/Admin/LoadHistoryOrders", {
        BranchIds: brnachIds,
        StartDate: OrderHistoryDatePicker.minRange.toJSON(),
        EndDate: OrderHistoryDatePicker.maxRange.toJSON(),

    }, successCallBack(successFunc, loader));
}

function jsonToDate(value) {
    let date;
    if (value.includes("/Date")) {
        date = new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10));
    } else {
        date = new Date(value);
    }

    return date;
}

function processingOrders(orders) {
    for (let order of orders) {
        processsingOrder(order);
    }

    return orders;
}

function processsingOrder(order) {
    order.Date = jsonToDate(order.Date);
    order.ProductCount = JSON.parse(order.ProductCountJSON);

    return order;
}

function toStringDateAndTime(date) {
    let day = date.getDate().toString();;
    day = day.length == 1 ? "0" + day : day;
    let month = (date.getMonth() + 1).toString();
    month = month.length == 1 ? "0" + month : month;
    let hours = date.getHours().toString();
    hours = hours.length == 1 ? "0" + hours : hours;
    let minutes = date.getMinutes().toString();
    minutes = minutes.length == 1 ? "0" + minutes : minutes;
    let dateStr = `${hours}:${minutes} ${day}.${month}.${date.getFullYear()}`;
    return dateStr;
}

function getParentItemFromOrdersDOM(e) {
    let $e = $(e);
    let cssClass = "order-item";
    let $parent = $e.parents(`.${cssClass}`);

    if ($parent.length == 0 &&
        $e.hasClass(cssClass)) {
        $parent = $e;
    }

    return $parent;
}

function getOrderIdFromItemOrdersDOM(e) {
    $parent = getParentItemFromOrdersDOM(e);

    return $parent.attr("order-id");
}

function getProductIdsForLoad(order) {
    let ids = [];

    for (let id in order.ProductCount) {
        ids.push(id);
    }

    return ids;
}

function isInteger(num) {
    return (num ^ 0) === num;
}

function getPriceValid(num) {
    if (!isInteger(num)) {
        num = num.toFixed(2);
    }

    return num;
}

var OrderProducts = {};
var OrderCategoryes = {};
function loadProductById(ids, callback) {
    $.post("/Admin/LoadOrderProducts", { ids: ids }, successCallBack(callback, null));
}

function getOrderById(orderId) {
    return baseGetOrderByIrd(Orders, orderId);
}

function baseGetOrderByIrd(collection, orderId) {
    let searchOrder = null;

    for (let order of collection) {
        if (order.Id == orderId) {
            searchOrder = order;
            break;
        }
    }

    return searchOrder;
}

function getHistoryOrderById(orderId) {
    return baseGetOrderByIrd(HistoryOrders, orderId);
}

function getCityNameById(id) {
    let $cityItem = $(`#setting-city-list [city-id=${id}]`);
    let cityName = "";

    if ($cityItem.length != 0) {
        cityName = $cityItem.val();
    }

    return cityName;
}

var BuyType = {
    Cash: 1,
    Card: 2
}

function getBuyType(id) {
    let buyType = "";

    switch (id) {
        case BuyType.Cash:
            buyType = "Наличные";
            break;
        case BuyType.Card:
            buyType = "Банковская карта";
            break;
    }

    return buyType;
}

var DeliveryType = {
    TakeYourSelf: 1,
    Delivery: 2
}

function getDeliveryType(id) {
    let deliveryType = "";

    switch (id) {
        case DeliveryType.TakeYourSelf:
            deliveryType = "Самовывоз";
            break;
        case DeliveryType.Delivery:
            deliveryType = "Доставка курьером";
            break;
    }

    return deliveryType;
}

const OrderStatus = {
    Processing: 0,
    Processed: 1,
    Cancellation: 2
}

function changeOrderStatus(orderId, orderStatus) {
    let $order = $(`#order .order-list-grid [order-id=${orderId}]`);

    $order.hide(500, function () {
        $(this.remove());
        for (let i = 0; i < Orders.length; ++i) {
            if (Orders[i].Id == orderId) {
                Orders.splice(i, 1);
                break;
            }
        }

        showCountOrder(Orders.length);

        if (Orders.length == 0) {
            setEmptyOrders();
        }
    });

    $.post("/Admin/UpdateSatsusOrder", { OrderId: orderId, Status: orderStatus }, successCallBack(() => getTodayDataOrders(Pages.Order)));
}

function searchByOrderNumber(containerId, isAnimation = true) {
    let searchNumber = $(`#${containerId} .search-input input`).val();
    $(`#${containerId} .order-item-grid`).each(function (index, e) {
        let $e = $(e);
        let orderNumber = $e.attr("order-id");

        if (orderNumber.includes(searchNumber)) {
            if (isAnimation)
                $e.show(500);
            else
                $e.show();
        } else {
            if (isAnimation)
                $e.hide(500);
            else
                $e.hide();
        }
    });

    if (containerId == Pages.HistoryOrder) {
        changeHistoryOrderDataForStatusBar();
    }
}

function showCountOrder(count) {
    let $orderCount = $(".order-count");
    if (count) {
        $orderCount.removeClass("hide");
        $orderCount.html(count);
    } else {
        $orderCount.addClass("hide");
    }

    OrderStatusBar.setCountNewOrder(count, Pages.Order);
}

var NotifySoundNewOrder = new Audio('../Content/sounds/sound-new-order.mp3');
NotifySoundNewOrder.autoplay = false;
NotifySoundNewOrder.stop = function () {
    if (!NotifySoundNewOrder.paused) {
        NotifySoundNewOrder.pause();
        NotifySoundNewOrder.currentTime = 0;
    }
}

function notifySoundNewOrder() {
    let soundOn = $("#sound-nodify").is(":checked");

    if (soundOn) {
        NotifySoundNewOrder.stop();
        NotifySoundNewOrder.play();
    }
}

function getTodayDataOrders(containerId) {
    let currentBranchId = getCurrentBranchId();
    let brnachIds = [...AdditionalBranch];
    brnachIds.push(currentBranchId);

    let successFunct = data => {
        setTodayDataOrders(data.Data, containerId)
    }

    $.post("/Admin/GetTodayOrderData", { brnachIds: brnachIds }, successCallBack(successFunct));
}

function changeHistoryOrderDataForStatusBar() {
    let historyOrderData = {
        CountSuccesOrder: 0,
        CountCancelOrder: 0,
        Revenue: 0
    }

    HistoryOrders.forEach(v => {
        if (v.OrderStatus == OrderStatus.Processed) {
            ++historyOrderData.CountSuccesOrder;
            historyOrderData.Revenue += v.AmountPayDiscountDelivery;
        } else {
            ++historyOrderData.CountCancelOrder;
        }
    })

    setTodayDataOrders(historyOrderData, Pages.HistoryOrder)
}

function setTodayDataOrders(data, containerId) {
    OrderStatusBar.setCountSuccessOrder(data.CountSuccesOrder, containerId);
    OrderStatusBar.setCountCancelOrder(data.CountCancelOrder, containerId);
    OrderStatusBar.setTodayRevenue(data.Revenue, containerId);
}

function getCurrentSectionId() {
    return $(".section:not(.hide)").attr("id")
}

class OrderStatusBar {

    static setCountNewOrder(value, containerId) {
        this.setValue(".count-new-order", value, containerId);
    }

    static setCountSuccessOrder(value, containerId) {
        this.setValue(".count-success-order", value, containerId);
    }

    static setCountCancelOrder(value, containerId) {
        this.setValue(".count-cancel-order", value, containerId);
    }

    static setTodayRevenue(value, containerId) {
        this.setValue(".today-revenue-order", getPriceValid(value), containerId);
    }

    static setValue(qSelector, value, containerId) {
        const selectorValueContainer = ".status-bar-value";
        $(`#${containerId} ${qSelector} ${selectorValueContainer}`).html(value);
    }
}

function showOrderDetails(orderId) {
    const currentSectionId = getCurrentSectionId();
    const order = currentSectionId == Pages.HistoryOrder ? getHistoryOrderById(orderId) : getOrderById(orderId);
    const getOrderDetails = () => {
        const orderDetailsData = new OrderDetailsData(order)

        return new OrderDetails(orderDetailsData);
    }


    let loader = new Loader($(`#${currentSectionId}`));
    loader.start()

    let productIdsforLoad = getProductIdsForLoad(order);

    let callbackLoadProducts = function (data) {
        if (data.Success) {
            OrderCategoryes = {};
            for (let categoryObj of data.Data) {
                OrderCategoryes[categoryObj.CategoryId] = {
                    Id: categoryObj.CategoryId,
                    Name: categoryObj.CategoryName,
                    ProductIds: categoryObj.Products.map(product => product.Id)
                };

                categoryObj.Products.forEach(product => OrderProducts[product.Id] = product);
            }

            loader.stop()
            getOrderDetails().show();
        } else {
            loader.stop()
            showErrorMessage(data.ErrorMessage);
        }
    }

    if (productIdsforLoad.length == 0) {
        loader.stop()
        getOrderDetails().show();
    } else {
        loadProductById(productIdsforLoad, callbackLoadProducts);
    }
}

var Dialog = {
    transition: null,
    showModal: ($dialog) => {
        $dialog = $($dialog);

        $dialog.trigger("showModal");
        this.transition = setTimeout(function () {
            $dialog.addClass('dialog-scale');
        }, 0.5);
    },
    close: ($dialog) => {
        $dialog = $($dialog);

        $dialog.trigger("close");
        $dialog.removeClass('dialog-scale');
        clearTimeout(this.transition);
    }
}

class CardOrder {
    /**
     *
     * @param {TodayOrder} cardData
     */
    constructor(cardData) {
        const templateId = "order-item-grid-template";
        this.data = cardData;
        this.$htmlTemplate = $($(`#${templateId}`).html());
    }

    setAttrOrderId(value) {
        this.$htmlTemplate.attr("order-id", value);
    }

    setOrderNumber(value, status) {
        this.setValue(".order-item-number", value);
        this.markNumberOrder(".order-item-number", status);
    }

    setAmount(value) {
        this.setValue(".order-item-amount", value);
    }

    setPhoneNumber(value) {
        this.setValue(".order-item-phone", value);
    }

    setUserName(value) {
        this.setValue(".order-item-user-name", value);
    }

    setDeliverType(value) {
        this.setValue(".order-item-type-delivery", value);
    }

    setPayType(value) {
        this.setValue(".order-item-type-pay", value);
    }

    setValue(qSelector, value) {
        const selectorValueContainer = ".order-item-value";
        this.$htmlTemplate.find(`${qSelector} ${selectorValueContainer}`).html(value);
    }

    setDetailsClickAction(action) {
        const eSenderContainer = ".order-item-show-details";
        const $eSender = $(this.$htmlTemplate.find(`${eSenderContainer} a`))

        $eSender.unbind("click");
        $eSender.bind("click", action);
    }

    markNumberOrder(qSelector, status) {
        const selectorLabelContainer = ".order-item-label";
        let label = "";

        switch (status) {
            case OrderStatus.Processed:
                label = StatusAtrr.Processed.numberOrderMark;
                break;
            case OrderStatus.Cancellation:
                label = StatusAtrr.Cancellation.numberOrderMark;
                break;
            default:
                label = StatusAtrr.Processing.numberOrderMark;
                break;
        }

        this.$htmlTemplate.find(`${qSelector} ${selectorLabelContainer}`).html(label);
    }

    render() {
        this.setAttrOrderId(this.data.OrderId);
        this.setOrderNumber(this.data.OrderNumber, this.data.Status);
        this.setAmount(this.data.Amount);
        this.setPhoneNumber(this.data.PhoneNumber);
        this.setUserName(this.data.UserName);
        this.setDeliverType(this.data.DeliveryType);
        this.setPayType(this.data.PayType);
        this.setDetailsClickAction(this.data.Action);

        return this.$htmlTemplate;
    }
}

class TodayOrder {
    constructor(order, action) {
        this.Action = () => { action(order.Id) };

        this.convert(order);
    }

    convert(order) {
        this.OrderId = order.Id;
        this.Status = order.OrderStatus;
        this.OrderNumber = order.Id;
        this.Amount = xFormatPrice(order.AmountPayDiscountDelivery);
        this.PhoneNumber = order.PhoneNumber;
        this.UserName = order.Name;
        this.DeliveryType = getDeliveryType(order.DeliveryType);
        this.PayType = getBuyType(order.BuyType);
    }
}

class CardOrderRenderer {
    static renderOrders(orders, containerId, speed) {
        let index = 0;
        for (let order of orders) {
            ++index

            if (index > 6) {
                speed = 1;
            }

            this.renderOrder(order, containerId, speed);
        }
    }

    static renderOrder(order, containerId, speed) {
        const todayData = new TodayOrder(order, showOrderDetails);
        const cardOrder = new CardOrder(todayData);

        this.addCardToPage(cardOrder, containerId, speed);
    }

    static addCardToPage(card, containerId, speed) {
        const cardContainer = ".order-list-grid";
        const cardRender = card.render()

        $(`#${containerId} ${cardContainer}`).append(cardRender);
        this.showCard(cardRender, speed);
    }

    static showCard(cardRender, speed) {
        if (speed == 1)
            $(cardRender).show();
        else
            $(cardRender).show(speed || 500);
    }
}

class OrderDetailsData {
    constructor(order) {
        this.convert(order);
    }

    convert(order) {
        this.convertBaseInfo(order);
        this.convertShortInfo(order);
        this.convertAmountInfo(order);
        this.convertAddressInfo(order);
        this.converеOrderListInfo(order);
    }

    convertBaseInfo(order) {
        const commentEmptyTemplate = `<i class="fal fa-comment-slash"></i>Отсутсвтует</span>`;
        this.OrderId = order.Id;
        this.OrderNumber = order.Id;
        this.Status = order.OrderStatus;
        this.OrderDate = toStringDateAndTime(order.Date);
        this.Comment = order.Comment || commentEmptyTemplate;
    }

    convertShortInfo(order) {
        this.UserName = order.Name;
        this.PhoneNumber = order.PhoneNumber;
        this.DeliveryType = getDeliveryType(order.DeliveryType);
    }

    convertAmountInfo(order) {
        const prefixRub = "руб.";
        const prefixPercent = "%";

        this.AmountPay = `${xFormatPrice(order.AmountPay)} ${prefixRub}`;
        this.DeliveryPrice = `${xFormatPrice(order.DeliveryPrice)} ${prefixRub}`;
        this.Discount = order.Discount == 0 ? `0${prefixPercent}` : `${order.Discount}${prefixPercent} (${xFormatPrice(order.AmountPay * order.Discount / 100)} ${prefixRub})`
        this.PayType = getBuyType(order.BuyType);
        this.CashBack = order.CashBack > 0 ? `${xFormatPrice(order.CashBack - order.AmountPayDiscountDelivery)} ${prefixRub}` : `${xFormatPrice(order.CashBack)} ${prefixRub}`;
        this.AmountPayDiscountDelivery = `${xFormatPrice(order.AmountPayDiscountDelivery)} ${prefixRub}`;
    }

    convertAddressInfo(order) {
        this.City = getCityNameById(order.CityId);
        this.Street = order.DeliveryType == DeliveryType.Delivery ? order.Street : $("#setting-street").val();
        this.House = order.DeliveryType == DeliveryType.Delivery ? order.HomeNumber : $("#setting-home").val();
        this.Apartament = order.DeliveryType == DeliveryType.Delivery ? order.ApartamentNumber : $("#setting-home").val();
        this.Level = order.DeliveryType == DeliveryType.Delivery ? order.Level : "-";
        this.IntercomCode = order.DeliveryType == DeliveryType.Delivery ? order.IntercomCode : "-";
        this.Entrance = order.DeliveryType == DeliveryType.Delivery ? order.EntranceNumber : "-";
    }

    converеOrderListInfo(order) {
        const prefixRub = "руб.";
        this.OrderList = [];

        for (let productId in order.ProductCount) {
            const product = OrderProducts[productId];
            const obj = {
                Image: product.Image,
                Name: product.Name,
                Price: `${order.ProductCount[productId]} x ${product.Price} ${prefixRub}`
            }

            const view = this.convertOrderProducrToView(obj);
            this.OrderList.push(view);
        }
    }

    convertOrderProducrToView(product) {
        return `
            <div class="order-details-product-item ">
                <div class="order-details-product-img">
                    <img src="${product.Image}">
                </div>
                <div class="order-details-product-name-price border-bottom">
                    <span>${product.Name}</span>
                    <span class="font-weight-bold grid-justify-self-flex-end">${product.Price}</span>
                </div>
             </div>
        `;
    }

}

var OrderDetailsQSelector = {
    OrderNumberBlock: ".order-details-number",//в истории заказов помечаем цветом оформленный или не оформленный ордер
    OrderNumber: ".order-details-number .value",
    OrderDate: ".order-details-date .value",
    UserName: ".order-details-short-user-name .value",
    PhoneNumber: ".order-details-short-phone .value",
    DeliveryType: ".order-details-short-delivery-type .value",
    PriceAmount: ".order-details-price-amount .value",
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
    CancelBtn: ".order-details-menu .btn-details-cancel"
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
    constructor(details) {
        const detailsDialogId = "orderDetailsDialog";
        this.$dialog = $(`#${detailsDialogId}`);
        this.details = details;
    }

    show() {
        this.setValues();
        this.buttonsConfig();
        Dialog.showModal(this.$dialog);
    }

    close() {
        Dialog.close(this.$dialog);
    }

    setValue(qSelectror, value) {
        this.$dialog.find(qSelectror).html(value);
    }

    setValues() {
        this.setBaseInfo();
        this.setShortInfo();
        this.setAmountInfo();
        this.setAddressInfo();
        this.setOrderListInfo();
        this.setComment();
    }

    markOrderNumberColorStatus(status) {
        const $block = this.$dialog.find(OrderDetailsQSelector.OrderNumberBlock);
        let colorClass = ""

        $block.removeClass(StatusAtrr.Processed.cssColorClass);
        $block.removeClass(StatusAtrr.Cancellation.cssColorClass);
        $block.removeClass(StatusAtrr.Processing.cssColorClass);

        switch (status) {
            case OrderStatus.Processed:
                colorClass = StatusAtrr.Processed.cssColorClass;
                break;
            case OrderStatus.Cancellation:
                colorClass = StatusAtrr.Cancellation.cssColorClass;
                break;
            default:
                colorClass = StatusAtrr.Processing.cssColorClass;
                break;
        }

        $block.addClass(colorClass);
    }

    setBaseInfo() {
        this.markOrderNumberColorStatus(this.details.Status);
        this.setValue(OrderDetailsQSelector.OrderNumber, this.details.OrderNumber);
        this.setValue(OrderDetailsQSelector.OrderDate, this.details.OrderDate);
    }

    setShortInfo() {
        this.setValue(OrderDetailsQSelector.UserName, this.details.UserName);
        this.setValue(OrderDetailsQSelector.PhoneNumber, this.details.PhoneNumber);
        this.setValue(OrderDetailsQSelector.DeliveryType, this.details.DeliveryType);
    }

    setAmountInfo() {
        this.setValue(OrderDetailsQSelector.PriceAmount, this.details.AmountPay);
        this.setValue(OrderDetailsQSelector.DeliveryPrice, this.details.DeliveryPrice);
        this.setValue(OrderDetailsQSelector.Discount, this.details.Discount);
        this.setValue(OrderDetailsQSelector.PayType, this.details.PayType);
        this.setValue(OrderDetailsQSelector.CashBack, this.details.CashBack);
        this.setValue(OrderDetailsQSelector.PriceToPay, this.details.AmountPayDiscountDelivery);
    }

    setAddressInfo() {
        this.setValue(OrderDetailsQSelector.City, this.details.City);
        this.setValue(OrderDetailsQSelector.Street, this.details.Street);
        this.setValue(OrderDetailsQSelector.House, this.details.House);
        this.setValue(OrderDetailsQSelector.Apartament, this.details.Apartament);
        this.setValue(OrderDetailsQSelector.Level, this.details.Level);
        this.setValue(OrderDetailsQSelector.IntercomCode, this.details.IntercomCode);
        this.setValue(OrderDetailsQSelector.Entrance, this.details.Entrance);
    }

    setOrderListInfo() {
        this.setValue(OrderDetailsQSelector.OrderList, this.details.OrderList);
    }

    setComment() {
        this.setValue(OrderDetailsQSelector.Comment, this.details.Comment);
    }

    buttonsConfig() {
        const $proccesed = $(OrderDetailsQSelector.ApplyBtn);
        const $cancel = $(OrderDetailsQSelector.CancelBtn);
        const actionOrder = (orderStatus) => () => {
            this.close();
            changeOrderStatus(this.details.OrderId, orderStatus)
        }

        $proccesed.unbind("click");
        $cancel.unbind("click");

        if (getCurrentSectionId() == Pages.HistoryOrder) {
            $proccesed.attr("disabled", true);
            $cancel.attr("disabled", true);
        } else {
            $proccesed.removeAttr("disabled");
            $cancel.removeAttr("disabled");
            $proccesed.bind("click", actionOrder(OrderStatus.Processed));
            $cancel.bind("click", actionOrder(OrderStatus.Cancellation));
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
            items.push(epmty);
        } else {
            for (let area of AreaDelivery) {
                let item = this.renderItem(area)

                items.push(item)
            }
        }

        this.setToPage(items)
    }

    setToPage(items) {
        $('.area-delivery-settings-list').html(items)
    }

    renderEmpty() {
        return `<div class="area-delivery-empty">Добавте районы доставки...</div>`
    }

    renderItem(areaDelivery) {
        const priceWithPrefix = `${areaDelivery.MinPrice} руб.`
        const actionEditClick = () => this.showEditDialog(areaDelivery.NameArea, areaDelivery.MinPrice, areaDelivery.UniqId)
        const actionRemoveClick = () => this.removeAreaDelivery(areaDelivery.UniqId)
        const template = `
            <div class="area-delivery-settings-item border-bottom">
                <span class="area-name">${areaDelivery.NameArea}</span>
                <span class="area-delivery-price">${priceWithPrefix}</span>
                <button class="area-delivery-settings-btn edit-btn"><i class="fal fa-edit"></i></button>
                <button class="area-delivery-settings-btn remove-btn"><i class="fal fa-trash-alt"></i></button>
            </div>`
        const $item = $(template)

        $item.find('.edit-btn').bind('click', actionEditClick)
        $item.find('.remove-btn').bind('click', actionRemoveClick)

        return $item
    }

    removeAreaDelivery(uniqId) {
        const loader = new Loader('#areaDeliveryEditDialog')
        loader.start()

        let newAreaDelivery = []

        for (let area of AreaDelivery) {
            if (area.UniqId != uniqId) {
                newAreaDelivery.push(area);
            }
        }

        AreaDelivery = newAreaDelivery

        this.render()
        loader.stop()
    }

    appendNewArea() {
        const uniqId = generateRandomString(10)

        this.showEditDialog('', '', uniqId)
    }

    showEditDialog(name, minPrice, uniqId) {
        $("#area-name").val(name)
        $("#area-price").val(minPrice)
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
        const uniqId = $("#area-uniqId").val()
        const areaDelivery = this.findAreaByUniqId(uniqId)

        if (!name || !minPrice) {
            showInfoMessage('Заполните все поля')
            loader.stop()
            return
        }

        if (areaDelivery) {
            areaDelivery.NameArea = name
            areaDelivery.MinPrice = minPrice
        } else {
            const newAreaDelivery = {
                UniqId: uniqId,
                NameArea: name,
                MinPrice: minPrice
            }

            AreaDelivery.push(newAreaDelivery)
        }

        this.render()
        loader.stop()
        Dialog.close('#areaDeliveryEditDialog')
    }

    findAreaByUniqId(uniqId) {
        for (let areaDelivery of AreaDelivery) {
            if (areaDelivery.UniqId === uniqId) {
                return areaDelivery
            }
        }

        return null;
    }
}