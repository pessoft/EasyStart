﻿$(document).ready(function () {

    let bindShowModal = function (id, dialogId) {
        $(`#${id}`).bind("click", function () {
            let addCategoryDialog = $(`#${dialogId}`);
            addCategoryDialog.trigger("showModal");
        });
    }

    $(".menu-item").not(".logout").bind("click", function () {
        SelectMenuItem(this);
    });

    bindShowModal("add-category", "addCategoryDialog");
    bindShowModal("add-product", "addProducDialog");
    bindShowModal("add-branch", "addBranchDialog");

    $("input[type=file]").change(function () {
        AddPreviewImage(this);
    });
});

function Logout() {
    let successFunc = (result) => {
        window.location.href = result.URL;
    }

    $.post("/Home/Logout", null, successCallBack(successFunc, null));
}

function SelectMenuItem(e) {
    let $e = $(e);
    let targetId = $e.attr("target-id");
    $(".menu-item").removeClass("menu-item-active");
    $e.addClass("menu-item-active");
    $(".section").addClass("hide");
    $(`#${targetId}`).removeClass("hide");
}

function CancelDialog(e) {
    let dialog = $(e);

    dialog.find("input").val("");
    dialog.find("textarea").val("");
    dialog.find(".dialog-image-upload").removeClass("hide");
    dialog.find("img").addClass("hide");
    dialog.trigger("close")
}

function AddCategory() {
    let category = {
        Name: $("#name-category").val(),
        Image: "image"
    }
    AddLoader($("#addCategoryDialog"));
    CancelDialog("#addCategoryDialog");
    $.post("/Admin/AddCategory", category, successCallBack(successFunc, null));
}

function AddProduct() {
    let product = {
        CategoryId: GetSelectedCategoryId(),
        Name: $("#name-product"),
        Price: $("#product-price"),
        Description: $("#description-product"),
        Image: "image"
    }
    AddLoader($("#addProducDialog"));
    CancelDialog("#addProducDialog");
    $.post("/Admin/AddProduct", product, successCallBack(successFunc, null));
}

function AddPreviewImage(input) {
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

function OpenDialogFile(id) {
    $(`#${id}`).click();
}

function AddLoader(e) {
    let loader = `<div class="loader"><img src="../images/loader.gif"/><div>`
    let form = $(e).find("form");

    RemoveLoader();
    form.append(loader);
}

function RemoveLoader(e) {
    let form = $(e).find("form");
    form.find(".loader").remove();
}

function SaveSetting() {

    let setting = {
        CityId: $("#setting-city-list option[value='" + $('#setting-city').val() + "']").attr('city-id'),
        Street: $("#setting-street").val(),
        HomeNumber: $("#setting-home").val(),
        PriceDelivery: $("#price-delivery").val(),
        FreePriceDelivery: $("#free-delivery").val(),
        TimeOpen: parseFloat($("#time-open").val()).toFixed(2).toString(),
        TimeClose: parseFloat($("#time-close").val()).toFixed(2),
    }

    let successFunc = function (result, loader) {
        if (result.Success) {
            alert("Настройка сохранена");
        } else {
            alert(result.ErrorMessage);
        }
    }

    $.post("/Admin/SaveSetting", setting, successCallBack(successFunc, null));
}

function AddBranch() {
    let newBranch = {
        Login: $("#"),
        Password: $("#"),
        CityId: $("#branch-city-list option[value='" + $('#branch-city').val() + "']").attr('city-id'),
    } 
    AddLoader($("#addBranchDialog"));
    CancelDialog($("#addBranchDialog"));
    $.post("/Admin/AddBranch", newBranch, successCallBack(successFunc, null));
}

function GetSelectedCategoryId() {
    return 0;
}