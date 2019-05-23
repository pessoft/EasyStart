﻿$(document).ready(function () {

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
            alert("Выберите категорию");
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
});

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
        alert("Выберите категорию")
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
                alert(result.ErrorMessage);
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
                alert(result.ErrorMessage);
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
            alert(result.ErrorMessage);
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
                alert(result.ErrorMessage);
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
                alert(result.ErrorMessage);
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
            alert(result.ErrorMessage);
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
                Data.Products.push(result.Data);
                addProductToList(result.Data);
                cancelDialog("#addProducDialog");
            } else {
                alert(result.ErrorMessage);
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
            productItem.find("img").attr("src", product.Image);
            productItem.find(".category-item-name").html(product.Name);
            productItem.find(".product-item-additional-info").html(product.AdditionInfo);
            productItem.find(".product-item-price span").html(product.Price);
            productItem.find(".product-item-description").html(product.Description);
            productItem.find(".product-type-item").html(product.ProductType);


            cancelDialog("#addProducDialog");
        } else {
            alert(result.ErrorMessage);
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
                alert(result.ErrorMessage);
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
                alert(result.ErrorMessage);
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
            alert(result.ErrorMessage);
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

    let setting = {
        Id: $("#setting").attr("setting-id"),
        CityId: $("#setting-city-list option[value='" + $('#setting-city').val() + "']").attr('city-id'),
        Street: $("#setting-street").val(),
        PhoneNumber: $("#setting-phone-number").val(),
        PhoneNumberAdditional: $("#setting-phone-number-additional").val(),
        Email: $("#setting-email").val(),
        HomeNumber: $("#setting-home").val(),
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
            alert(result.ErrorMessage);
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

        timeDays[DayWeekly[day]] = checked ? [start, end] : null;
    }

    return JSON.stringify(timeDays);
}

function saveDeliverySetting() {

    let setting = {
        PriceDelivery: $("#price-delivery").val(),
        FreePriceDelivery: $("#free-delivery").val(),
        ZoneId: $("#delivery-time-zone").val(),
        PayCard: $("#payment-card").is(":checked"),
        PayCash: $("#payment-cash").is(":checked"),
        TimeDeliveryJSON: getTimeDeliveryJSON(),
    }
    let loader = new Loader($("#delivery"));
    let successFunc = function (result, loader) {
        loader.stop();
        if (!result.Success) {
            alert(result.ErrorMessage);
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
            alert(result.ErrorMessage);
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
            alert(result.ErrorMessage);
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
            alert(result.ErrorMessage);
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
            alert(result.ErrorMessage);
        }

        if (callback) {
            callback();
        }
    }

    $.post("/Admin/LoadProductReviews", { productId: productId }, successCallBack(successFunc));
}