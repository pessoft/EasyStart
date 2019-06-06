$(document).ready(function () {

    let bindShowModal = function (id, dialogId, additionalFunc, predicate) {
        $(`#${id}`).bind("click", function () {
            let addCategoryDialog = $(`#${dialogId}`);

            if (!predicate || predicate()) {
                addCategoryDialog.trigger("showModal");
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

    if ($(".header-menu .menu-item-active").attr("target-id") == "order") {
        loadOrders();
    }

    bindSelectSumo();
});

var AdditionalBranch = [];
function bindSelectSumo() {
    $("#show-additional-order").SumoSelect({
        okCancelInMulti: true,
        placeholder: 'Выберите города'
    });

    let bindFunc = function () {
        AdditionalBranch = [];
        $('#show-additional-order option:selected').each(function (i) {
            AdditionalBranch.push($(this).attr("key"));
        });

        loadOrders(true);
    }

    bindSumoOk(bindFunc);
}

function bindSumoOk(func) {
    $(".additional-order .btnOk").bind("click", func)
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

function selectMenuItem(e) {
    let $e = $(e);
    let targetId = $e.attr("target-id");
    $(".menu-item").removeClass("menu-item-active");
    $e.addClass("menu-item-active");
    $(".section").addClass("hide");
    $(`#${targetId}`).removeClass("hide");

    if (targetId == "stock") {
        loadStockList();
    }
}

function cancelDialog(e) {
    let dialog = $(e);

    dialog.find("input").val("");
    dialog.find("textarea").val("");
    dialog.find(".dialog-image-upload").removeClass("hide");
    dialog.find("img").addClass("hide");
    dialog.find("option").removeAttr("selected");
    dialog.find("select").val("0")
    dialog.trigger("close")
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

    dialog.trigger("showModal");
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
    let category = {
        Id: $("#category-id").val(),
        Name: $("#name-category").val(),
        Image: $("#addCategoryDialog img").attr("src")
    }
    let loader = new Loader($("#addCategoryDialog form"));
    let successFunc = function (result, loader) {
        loader.stop();
        if (result.Success) {
            let categoryItem = $(`[category-id=${category.Id}]`);
            categoryItem.find("img").attr("src", category.Image);
            categoryItem.find(".category-item-name").html(category.Name);

            cancelDialog("#addCategoryDialog");
        } else {
            showErrorMessage(result.ErrorMessage);
        }
    }
    loader.start();

    $.post("/Admin/UpdateCategory", category, successCallBack(successFunc, loader));
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

    dialog.trigger("showModal");
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
                //Data.Products.push(result.Data);
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
    let product = {
        Id: $("#product-id").val(),
        CategoryId: SelectIdCategoryId,
        Name: $("#name-product").val(),
        AdditionInfo: $("#product-additional-info").val(),
        Price: $("#product-price").val(),
        Description: $("#description-product").val(),
        Image: $("#addProducDialog img").attr("src"),
        ProductType: parseInt($("#product-type option:selected").attr("value"))
    }
    let loader = new Loader($("#addProducDialog form"));
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
            productItem.find(".category-item-name").html(product.Name);
            productItem.find(".product-item-additional-info").html(product.AdditionInfo);
            productItem.find(".product-item-price span").html(product.Price);
            productItem.find(".product-item-description").html(product.Description);
            productItem.find(".product-type-item").html(product.ProductType);


            cancelDialog("#addProducDialog");
        } else {
            showErrorMessage(result.ErrorMessage);
        }
    }
    loader.start();

    $.post("/Admin/UpdateProduct", product, successCallBack(successFunc, loader));
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

    dialog.trigger("showModal");
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

    $dialog.trigger("showModal");
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

function checkRequreValid() {

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
        FreePriceDelivery: "Укажите минимальную сумму заказа для бесплатной доставки",
    }

    let priceDelivery = $("#price-delivery").val();
    let freePriceDelivery = $("#free-delivery").val();

    if (!priceDelivery) {
        showWarningMessage(warnMgs.PriceDelivery);
        return;
    }
    if (!freePriceDelivery || freePriceDelivery == 0) {
        showWarningMessage(warnMgs.FreePriceDelivery);
        return;
    }

    let setting = {
        PriceDelivery: priceDelivery,
        FreePriceDelivery: freePriceDelivery,
        ZoneId: $("#delivery-time-zone").val(),
        PayCard: $("#payment-card").is(":checked"),
        PayCash: $("#payment-cash").is(":checked"),
        TimeDeliveryJSON: getTimeDeliveryJSON(),
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

        $dialog.trigger("showModal");

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


function setEmptyOrders() {
    let template = `
        <div class="empty-list">
            <i class="fal fa-user-clock"></i>
            <span>Пока нет ни одного заказа</span>
        </div>
    `;

    $("#order .order-list").append(template);
}

function setEmptyOrderInfo() {
    let template = `
        <div class="empty-list">
            <i class="fal fa-info"></i>
            <span>Что бы увидеть информацию о заказе, выберите нужный из колонки "Заказы"</span>
        </div>
    `;

    $("#order .order-description").append(template);
}


var Orders = [];

function loadOrders(reload = false) {
    if (Orders.length != 0 && !reload) {
        return;
    }

    currentBranchId = $("#current-brnach").val();
    let brnachIds = getAdditionBranches();
    brnachIds.push(currentBranchId);

    let $list = $("#order .orders .order-list");
    $list.empty();

    let loader = new Loader($("#order"));
    loader.start();

    let successFunc = function (data, loader) {
        if (data.Success) {
            Orders = processingOrders(data.Data);

            if (Orders.length == 0) {
                setEmptyOrders();
            } else {
                renderOrders();
            }
        } else {
            showErrorMessage(data.ErrorMessage);
        }

        loader.stop();
    }

    $.post("/Admin/LoadOrders", { brnachIds: brnachIds }, successCallBack(successFunc, loader));
}

function getAdditionBranches() {
    return AdditionalBranch;
}

function renderOrders() {
    let templates = [];

    for (let order of Orders) {
        templates.push(getTemplateOrderItem(order));
    }

    $("#order .order-list").html(templates);
}

function getTemplateOrderItem(data) {
    let $template = $($(`#order-item-template-${data.DeliveryType}`).html());
    let orderNumber = `№ ${data.Id}`;
    let date = toStringDateAndTime(data.Date);

    $template.attr("order-id", data.Id);
    $template.find(".order-item-number span").html(orderNumber);
    $template.find(".order-item-date").html(date);

    $template.bind("click", function () { selectOrder(this) });

    return $template;
}

function jsonToDate(value) {
    date = new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10));

    return date;
}

function processingOrders(orders) {
    for (let order of orders) {
        order.Date = jsonToDate(order.Date);
        order.ProductCount = JSON.parse(order.ProductCountJSON);
    }

    return orders;
}

function toStringDateAndTime(date) {
    let day = date.getDate().toString();;
    day = day.length == 1 ? "0" + day : day;
    let month = (date.getMonth() + 1).toString();
    month = month.length == 1 ? "0" + month : month;
    let dateStr = `${date.getHours()}:${date.getMinutes()} ${day}.${month}.${date.getFullYear()}`;
    return dateStr;
}

function setActiveOrderItem(e) {
    $parent = getParentItemFromOrdersDOM(e);

    $("#order .order-item-active").removeClass("order-item-active");
    $parent.addClass("order-item-active");
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

function selectOrder(e) {
    let orderId = getOrderIdFromItemOrdersDOM(e);

    setActiveOrderItem(e);
    clearOrderDescriptionBlock();
    showInfoDesctiprionOrder(orderId);

    $("#order .order-description .empty-list").remove();
}

function clearOrderDescriptionBlock() {
    $(".order-description-number-header").empty();
    $(".order-description-date-header").empty();
    $(".order-description-products").empty();
    $("#order .to-pay .order-description-additional-info-value").empty();

    $("#order .need-cashback .order-description-additional-info-value").empty();
    $("#order .need-cashback").addClass("hide");

    $("#order .order-user-name .order-description-additional-info-value").empty();
    $("#order .order-user-phone .order-description-additional-info-value").empty();
    $(".order-description-info-take").empty();

    $("#order-description-comment .order-description-info-comment-text").empty();
    $("#order-description-comment").addClass("hide");
}

function showInfoDesctiprionOrder(orderId) {
    let order = getOrderById(orderId);
    let loader = new Loader($("#order .order-description"));

    loader.start()

    let productIdsforLoad = getProductIdsForLoad(order);

    let callbackLoadProducts = function (data) {
        if (data.Success) {
            for (let product of data.Data) {
                OrderProducts[product.Id] = product;
            }

            callbackShowDescription();
        } else {
            showErrorMessage(data.ErrorMessage);
            loader.stop()
        }
    }

    let callbackShowDescription = function () {
        let dataStr = getDataOrderStr(order);

        renderOrderNumberHeader(dataStr.orderNumber, dataStr.date);
        renderorderOrderProducts(dataStr.products);
        renderOrderAdditionaInfo(dataStr);
        renderOrderInfoTake(dataStr);
        renderOrderComment(dataStr.comment);
        bindUpdateStatusButton(order);
        loader.stop()
    }

    if (productIdsforLoad.length == 0) {
        callbackShowDescription();
    } else {
        loadProductById(productIdsforLoad, callbackLoadProducts);
    }
}

function bindUpdateStatusButton(order) {
    let $proccesed = $("#order .btn-submit");
    let $cancel = $("#order .btn-cancel-order");

    $proccesed.unbind("clcik");
    $cancel.unbind("clcik");

    $proccesed.bind("click", () => changeOrderStatus(order.Id, OrderStatus.Processed));
    $cancel.bind("click", () => changeOrderStatus(order.Id, OrderStatus.Cancellation));
}

function renderOrderComment(comment) {
    if (comment) {
        let $comment = $("#order-description-comment");

        $comment.removeClass("hide");
        $comment.find(".order-description-info-comment-text").html(comment);
    }
}

function renderorderOrderProducts(dataProducdtsStr) {
    let templates = [];

    for (let product of dataProducdtsStr) {
        let $template = $($("#order-product-item-template").html());

        $template.find(".order-description-product-number").html(product.Index);
        $template.find(".order-description-product-name").html(product.Name);
        $template.find(".order-description-product-info-count").html(product.Count);
        $template.find(".order-description-product-info-price").html(product.Price);

        templates.push($template);
    }

    $("#order .order-description-products").html(templates);
}

function renderOrderAdditionaInfo(dataStr) {
    $("#order .to-pay .order-description-additional-info-value").html(dataStr.amountPayDiscountDelivery);
    $("#order #need-cashback-header").html(dataStr.cashBack);
    $("#order .need-cashback .order-description-additional-info-value").html(dataStr.cashBackNumber);
    $("#order .order-user-name .order-description-additional-info-value").html(dataStr.name);
    $("#order .order-user-phone .order-description-additional-info-value").html(dataStr.phoneNumber);

    if (dataStr.needCashBack) {
        $("#order .need-cashback").removeClass("hide");
    }
}

function renderOrderNumberHeader(numberOrder, date) {
    $("#order .order-description-number-header").html(numberOrder);
    $("#order .order-description-date-header").html(date);
}

function renderOrderInfoTake(dataStr) {
    let $payInfo = $($("#info-order-cost-template").html());
    let $deliveryInfo = dataStr.deliveryType == DeliveryType.Delivery ?
        $($("#info-order-delivery-template").html()) :
        $($("#info-order-takeyorself-template").html());

    $payInfo.find("#take-info-pay-type .take-info-item-value").html(dataStr.buyType);
    $payInfo.find("#take-info-price .take-info-item-value").html(dataStr.amountPay);
    $payInfo.find("#take-info-discount .take-info-item-value").html(dataStr.discount);
    $payInfo.find("#take-info-delivery .take-info-item-value").html(dataStr.deliveryPrice);
    $payInfo.find("#take-info-topay .take-info-item-value").html(dataStr.amountPayDiscountDelivery);

    $deliveryInfo.find("#order-takeyorself-city .take-info-item-value").html(dataStr.city);
    $deliveryInfo.find("#order-takeyorself-street .take-info-item-value").html(dataStr.street);
    $deliveryInfo.find("#order-takeyorself-home .take-info-item-value").html(dataStr.homeNumber);

    if (dataStr.deliveryType == DeliveryType.Delivery) {
        $deliveryInfo.find("#order-delivery-city .take-info-item-value").html(dataStr.city);
        $deliveryInfo.find("#order-delivery-street .take-info-item-value").html(dataStr.street);
        $deliveryInfo.find("#order-delivery-home .take-info-item-value").html(dataStr.homeNumber);
        $deliveryInfo.find("#order-delivery-apartment .take-info-item-value").html(dataStr.apartamentNumber);
        $deliveryInfo.find("#order-delivery-intercom-code .take-info-item-value").html(dataStr.intercomCode);
    }

    let $descriptionItemTake = $("#order .order-description-info-take");

    $descriptionItemTake.append($payInfo);
    $descriptionItemTake.append($deliveryInfo);
}

function getProductIdsForLoad(order) {
    let ids = [];

    for (let id in order.ProductCount) {
        if (!OrderProducts[id]) {
            ids.push(id);
        }
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

function getDataOrderStr(order) {
    let prefixRub = "руб.";
    let prefixPercent = "%";
    let prefixCount = "шт.";
    let getProductsStr = () => {
        let products = [];
        let index = 0;
        for (let productId in order.ProductCount) {
            let product = OrderProducts[productId];
            let obj = {
                Index: `${++index}.`,
                Name: product.Name,
                Count: `${order.ProductCount[productId]} ${prefixCount}`,
                Price: `${product.Price} ${prefixRub}`
            }

            products.push(obj);
        }

        return products;
    };

    let street = order.DeliveryType == DeliveryType.Delivery ? order.Street : $("#setting-street").val();
    let homeNumber = order.DeliveryType == DeliveryType.Delivery ? order.HomeNumber : $("#setting-home").val();
    let deliveryPrice = order.DeliveryPrice == 0 ? "Бесплатно" : `${getPriceValid(order.DeliveryPrice)} ${prefixRub}`;
    let discount = order.Discount == 0 ? `0${prefixPercent}` : `${order.Discount}${prefixPercent} (${getPriceValid(order.AmountPay * order.Discount / 100)} ${prefixRub})`
    let dataStr = {
        amountPay: `${getPriceValid(order.AmountPay)} ${prefixRub}`,
        amountPayDiscountDelivery: `${getPriceValid(order.AmountPayDiscountDelivery)} ${prefixRub}`,
        apartamentNumber: order.ApartamentNumber,
        buyType: getBuyType(order.BuyType),
        cashBack: `Нужна сдача с ${order.CashBack} ${prefixRub}:`,
        cashBackNumber: `${getPriceValid(order.CashBack - order.AmountPayDiscountDelivery)} ${prefixRub}`,
        city: getCityNameById(order.CityId),
        comment: order.Comment,
        date: toStringDateAndTime(order.Date),
        deliveryType: order.DeliveryType,
        discount: discount,
        entranceNumber: order.EntranceNumber,
        homeNumber: homeNumber,
        orderNumber: `№ ${order.Id}`,
        intercomCode: order.IntercomCode,
        level: order.Level,
        name: order.Name,
        needCashBack: order.NeedCashBack,
        phoneNumber: order.PhoneNumber,
        products: getProductsStr(),
        street: street,
        deliveryPrice: deliveryPrice
    }

    return dataStr;
}

var OrderProducts = {}
function loadProductById(ids, callback) {
    $.post("/Admin/LoadOrderProducts", { ids: ids }, successCallBack(callback, null));
}

function getOrderById(orderId) {
    let searchOrder = null;

    for (let order of Orders) {
        if (order.Id == orderId) {
            searchOrder = order;
            break;
        }
    }

    return searchOrder;
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
        case BuyType.Delivery:
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
    let $order = $(`#order [order-id=${orderId}]`);

    $order.fadeOut(600, function () {
        $(this.remove());
        for (let i = 0; i < Orders.length; ++i) {
            if (Orders[i].Id == orderId) {
                Orders.splice(i, 1);
                break;
            }
        }

        if (Orders.length == 0) {
            setEmptyOrders();
        }
    });

    setEmptyOrderInfo();
    $.post("/Admin/UpdateSatsusOrder", { OrderId: orderId, Status: orderStatus }, null);
}