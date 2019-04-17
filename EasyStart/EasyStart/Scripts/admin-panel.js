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

    $(".menu-item").not(".logout").bind("click", function () {
        selectMenuItem(this);
    });

    bindShowModal("add-category", "addCategoryDialog");
    bindShowModal("add-product", "addProducDialog");
    bindShowModal("add-branch", "addBranchDialog", addNewBranchLoginData);

    $("input[type=file]").change(function () {
        addPreviewImage(this);
    });
});

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

function addCategory() {
    let category = {
        Name: $("#name-category").val(),
        Image: "image"
    }
    let loader = new Loader($("#addCategoryDialog form"));
    let successFunc = function (result, loader) {
        loader.stop();
        if (result.Success) {
            cancelDialog("#addCategoryDialog");
        } else {
            alert(result.ErrorMessage);
        }
    }

    loader.start();
    
    $.post("/Admin/AddCategory", category, successCallBack(successFunc, loader));
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
        HomeNumber: $("#setting-home").val(),
        PriceDelivery: $("#price-delivery").val(),
        FreePriceDelivery: $("#free-delivery").val(),
        TimeOpen: parseFloat($("#time-open").val()).toFixed(2).toString(),
        TimeClose: parseFloat($("#time-close").val()).toFixed(2),
    }
    let loader = new Loader($("#setting"));
    let successFunc = function (result, loader) {
        loader.stop();
        if (result.Success) {
            alert("Настройка сохранена");
        } else {
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
    let templateBranchItem = `<div class="branch-item">
        <div class="branch-item-info">
            <div class="branch-adress">${branchView.Addres}</div>
            <div class="branch-operation-mode">${branchView.OperationMode}</div>
            <div class="branch-phone-number">${branchView.PhoneNumber}</div>
            <div class="branch-login">${branchView.Login}</div>
            <div class="branch-password">${branchView.Password}</div>
        </div>
        <div class="branch-item-action ${branchView.Login == '******' ? 'disbled' : ''})">
            <i class="fas fa-trash-alt"></i>
        </div>
    </div >`;

    $(".branch-list").append(templateBranchItem);
}

function getSelectedCategoryId() {
    return 0;
}