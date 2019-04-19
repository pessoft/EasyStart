$(document).ready(function () {

    let bindShowModal = function (id, dialogId, additionalFunc) {
        $(`#${id}`).bind("click", function () {
            let addCategoryDialog = $(`#${dialogId}`);
            addCategoryDialog.trigger("showModal");

            if (additionalFunc) {
                additionalFunc();
            }
        });
    }

    $("#setting-phone-number").mask("+7(999)999-99-99");

    $(".menu-item").not(".logout").bind("click", function () {
        selectMenuItem(this);
    });

    let setOperationAdd = () => CurrentOperation = TypeOperation.Add;

    bindShowModal("add-category", "addCategoryDialog", setOperationAdd);
    bindShowModal("add-product", "addProducDialog", setOperationAdd);
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
    dialog.trigger("close")
}

function operatinCategory() {
    switch (CurrentOperation) {
        case TypeOperation.Add:
            addCategory();
            break;
        case TypeOperation.Update:
            updateCategory();
            break;
    }
}

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
    let category = {
        Name: $("#name-category").val(),
        Image: $("#addCategoryDialog img").attr("src")
    }
    let loader = new Loader($("#addCategoryDialog form"));
    let successFunc = function (result, loader) {
        loader.stop();
        if (result.Success) {
            addCategoryToList(result.Data);
            cancelDialog("#addCategoryDialog");
        } else {
            alert(result.ErrorMessage);
        }
    }

    loader.start();
    
    $.post("/Admin/AddCategory", category, successCallBack(successFunc, loader));
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
            <i onclick="editCategory(this, event);" class="far fa-edit"></i>
            <i onclick="removeCategory(this, event);" class="fas fa-trash-alt"></i>
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

    dialog.find("img").removeClass("hide");
    dialog.find(".dialog-image-upload").addClass("hide");

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
            }
                
            $(`[category-id=${id}]`).fadeOut(500, function () {
                $(this).remove();
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
    SelectIdCategoryId = $(e).attr("category-id");
}

function loadProducts() {
    loadCategoryList();
}

var SelectIdCategoryId;
function loadCategoryList() {
    SelectIdCategoryId = null;
    let container = $(".category-list");
    container.empty();
    let loader = new Loader(container);
    let successFunc = function (result, loader) {
        loader.stop();
        if (result.Success) {
            for (let category of result.Data) {
                addCategoryToList(category);
            }

        } else {
            alert(result.ErrorMessage);
        }
    }
    loader.start();

    $.post("/Admin/LoadCategoryList", null, successCallBack(successFunc, loader));
}

function addProduct() {
    let product = {
        CategoryId: GetSelectedCategoryId(),
        Name: $("#name-product"),
        Price: $("#product-price"),
        Description: $("#description-product"),
        Image: "image"
    }
    let loader = new Loader($("#addProducDialog  form"));
    let successFunc = function (result, loader) {
        loader.stop();
        if (result.Success) {
            cancelDialog("#addProducDialog");
        } else {
            alert(result.ErrorMessage);
        }
    }
    loader.start();

    $.post("/Admin/AddProduct", product, successCallBack(successFunc, loader));
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
        HomeNumber: $("#setting-home").val(),
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
    <div class="branch-item branch-id="${branchView.Id}"> 
        <div class="branch-item-info">
            <div class="branch-adress">${branchView.Addres}</div>
            <div class="branch-operation-mode">${branchView.OperationMode}</div>
            <div class="branch-phone-number">${branchView.PhoneNumber}</div>
            <div class="branch-login">${branchView.Login}</div>
            <div class="branch-password">${branchView.Password}</div>
        </div>
        <div onclick="removeBranch(this, ${branchView.Id})" class="branch-item-action ${branchView.Login.indexOf('******') != -1 ? 'disbled' : ''}">
            <i class="fas fa-trash-alt"></i>
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

function getSelectedCategoryId() {
    return 0;
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

    $.post("/Admin/RemoveBranch", { id: id}, successCallBack(successFunc, loader));
}