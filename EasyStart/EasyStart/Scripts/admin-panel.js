$(document).ready(function () {
    $(".menu-item").not(".logout").bind("click", function () {
        SelectMenuItem(this);
    });

    $("#add-category").bind("click", function () {
        let addCategoryDialog = $("#addCategoryDialog");
        addCategoryDialog.trigger("showModal");
    });

    $("#add-product").bind("click", function () {
        let addProducDialog = $("#addProducDialog");
        addProducDialog.trigger("showModal");
    });

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
    let dialog = $(e).parents("dialog");

    dialog.find("input").val("");
    dialog.find("textarea").val("");
    dialog.find(".dialog-image-upload").removeClass("hide");
    dialog.find("img").addClass("hide");
    dialog.trigger("close")
}

function AddCategory() {
    let nameCategory = $("#name-category").val();
    AddLoader($("#addCategoryDialog"));
    CancelDialog($("#name-category"));
}

function AddProduct() {
    AddLoader($("#addProducDialog"));
    CancelDialog($("#name-category"));
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