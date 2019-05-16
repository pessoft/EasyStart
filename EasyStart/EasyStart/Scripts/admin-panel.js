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
            alert("Выберите категорию");
            return false;
        }

        return true;
    };
    bindShowModal("add-category", "addCategoryDialog", setOperationAdd);
    bindShowModal("add-product", "addProducDialog", setOperationAdd, productPredicate);
    bindShowModal("add-branch", "addBranchDialog", addNewBranchLoginData);

    $("input[type=file]").change(function () {
        addPreviewImage(this);
    });
});

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
}

function cancelDialog(e) {
    let dialog = $(e);

    dialog.find("input").val("");
    dialog.find("textarea").val("");
    dialog.find(".dialog-image-upload").removeClass("hide");
    dialog.find("img").addClass("hide");
    dialog.find("option").removeAttr("selected");
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
                $(".empty-list").remove();
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

function loadCategoryList() {
    SelectIdCategoryId = null;
    clearCategoryList();

    let container = $(".category-list");
    let loader = new Loader($(".category"));
    let successFunc = function (result, loader) {
        loader.stop();
        if (result.Success) {
            if (!result.Data || result.Data.length == 0) {
                setEmptyCategoryInfo();
            } else {
                for (let category of result.Data) {
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
                $(".empty-list").remove();
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
            <div class="product-item-additional-info">
                ${product.AdditionInfo}
            </div>
            <div class="product-item-price">
                <span>${product.Price}</span>
                <i class="fal fa-ruble-sign"></i>
            </div>
            <div class="product-item-action">
                <i onclick="editProduct(this, event);" class="fal fa-edit"></i>
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

    $(".product-list").append(templateCategoryItem);
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

function removeProduct(e, event) {
    event.stopPropagation();
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
                for (let product of result.Data) {
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