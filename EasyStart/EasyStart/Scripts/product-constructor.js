function changeIngredientType(radioId) {
    $(`#${radioId}`).click()
}

const StyleTypeIngredient = {
    Short: 0,
    Long: 1
}

var categoryIngredient = null

function getIngredientData(image) {
    return {
        Id: parseInt($('#product-ingredient-id').val()),
        Name: $('#name-product-ingredient').val(),
        AdditionaInfo: $('#product-ingredient-additional-info').val(),
        Price: $('#product-ingredient-price').val(),
        MinRequiredCount: $('#product-min-ingredient-count').val(),
        MaxAddCount: $('#product-ingredient-max-count').val(),
        Description: $('#ingredient-description-product').val(),
        Image: image,
        TMPUniqId: $('#product-ingredient-tmp-uniqId').val()

    }
}

function addOrUpdateIngredient() {
    let loader = new Loader($("#addProducIngredientDialog form"));
    loader.start();

    let files = $("#addProducIngredientDialog input[type=file]")[0].files;
    var dataImage = new FormData();

    for (var x = 0; x < files.length; x++) {
        dataImage.append("file" + x, files[x]);
    }

    let addFunc = (data) => {
        let ingredient = getIngredientData(data.URL)

        categoryIngredient.addTMPImageURL(data.URL)
        categoryIngredient.addOrUpdateIngredient(ingredient)
        categoryIngredient.setData()

        loader.stop()
        Dialog.close('#addProducIngredientDialog')
        Dialog.clear()
    }

    if (files.length == 0) {
        let data = {
            URL: $("#addProducIngredientDialog img").attr("src")
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


function saveCategoruConstructor() {
    categoryIngredient.save()
}

function showDialogAddNewIngredient() {
    initDialogDefaultId()
    Dialog.showModal('#addProducIngredientDialog')
}

function initDialogDefaultId() {
    $('#product-ingredient-id').val(-1)
    $('#product-ingredient-tmp-uniqId').val(generateRandomString(16))
}

function initNewCategoryConstructor() {
    categoryIngredient = new CategoryIngredient()
}

class CategoryIngredient {
    constructor(categoryIngredient) {
        let defaultCategoryIngredient = {
            Category: {
                Id: -1,
                Name: '',
                MaxCountIngredient: 0,
                StyleTypeIngredient: StyleTypeIngredient.Short,
            },
            Ingredients: []
        }
        this.categoryIngredient = categoryIngredient && Object.keys(categoryIngredient).length > 0 ?
            categoryIngredient :
            defaultCategoryIngredient
        this.tmpURL = []
        this.categoryIngredientOrigin = { ...this.categoryIngredient }
        const reducer = (acc, value, index) => acc[value.Id] = index
        this.indexIngredients = this.categoryIngredient.Ingredients.length > 0 ?
            this.categoryIngredient.Ingredients.reduce(reducer) :
            {}
    }

    addOrUpdateIngredient(ingredient) {
        if (ingredient.Id > 0) {
            if (this.indexIngredients[ingredient.Id]) {
                const index = this.indexIngredients[ingredient.Id]
                this.categoryIngredient.Ingredients[index] = ingredient
            }
        } else {
            const index = this.categoryIngredient.Ingredients.indexOf(ingredient)

            if (index == -1) {
                this.categoryIngredient.Ingredients.push(ingredient)
            } else {
                this.categoryIngredient.Ingredients[index] = ingredient
            }
        }

    }

    save() {
        if (this.categoryIngredient.Ingredients
            && this.categoryIngredient.Ingredients.length > 0) {

            let loader = new Loader($("#addSubCategoryConstructorDialog form"));
            loader.start();

            let category = {
                id: this.categoryIngredient.Category.Id,
                name: this.categoryIngredient.Category.Name,
                maxCountIngredient: this.categoryIngredient.Category.MaxCountIngredient,
                styleTypeIngredient: this.categoryIngredient.Category.StyleTypeIngredient,
                ingredients: this.categoryIngredient.Ingredients
            }


            let successFunc = function (result, loader) {
                loader.stop();
                if (result.Success) {
                    this.categoryIngredient = result.Data
                    this.categoryIngredientOrigin = { ...result.Data }
                    this.tmpURL = []

                    this.clearIngredientRows()
                    this.setData()
                } else {
                    showErrorMessage(result.ErrorMessage);
                }
            }

            $.post("/Admin/AddCategoryConstructor", category, successCallBack(successFunc, loader));
        }
    }

    setEmptyList() {
        $('#ingrediet-list').append(`
            <div class="empty-list no-background">
                <i class="fal fa-smile-plus"></i>
                <span>Добавте ингредиенты</span>
            </div>`)
    }

    removeEmptyList() {
        $('#ingrediet-list .empty-list').remove()
    }

    setData() {
        this.removeEmptyList()
        this.setIngredientsParams()
        this.setIngredientList()
    }

    setIngredientsParams() {
        $('category-constructor-name').val(this.categoryIngredient.Category.Name)
        $('category-constructor-max-count').val(this.categoryIngredient.Category.MaxCountIngredient)

        switch (this.categoryIngredient.Category.StyleTypeIngredient) {
            case StyleTypeIngredient.Short:
                changeIngredientType('ingredient-view-type-short')
                break
            case StyleTypeIngredient.Long:
                changeIngredientType('ingredient-view-type-long')
                break;
        }
    }

    setIngredientList() {
        const ingredientRows = []

        for (const ingredient of this.categoryIngredient.Ingredients) {
            ingredientRows.push(this.renderIngredientRow(ingredient))
        }

        if (ingredientRows.length > 0)
            this.appendIngredientRows(ingredientRows)
        else
            this.setEmptyList()
    }

    renderIngredientRow(ingredient) {
        return `
        <div class="ingredient-row">
            <img src="${ingredient.Image}" alt="ingredient image" />
            <div class="product-ingredient-wrapper">
                <span class="ingredient-row-name">${ingredient.Name}</span>
                <span class="ingredient-row-small-text">${ingredient.AdditionaInfo}</span>
                <span class="ingredient-row-small-text">${ingredient.Price} руб.</span>
                <div class="ingredient-row-menu">
                    <i onclick="" class="fal fa-edit"></i>
                    <i onclick="" class="fal fa-trash-alt"></i>
                </div>
            </div>
        </div>
        `
    }

    clearIngredientRows() {
        $('#ingrediet-list').empty()
    }

    appendIngredientRows(ingredientRows) {
        $('#ingrediet-list').append(ingredientRows)
    }

    revert() {
        this.categoryIngredient = { ...this.categoryIngredientOrigin }
        this.actionRemoveTMPImage()
        this.setData()
    }

    addTMPImageURL(url) {
        this.tmpURL.push(url)
    }

    actionRemoveTMPImage() {
        //send url to remove
    }
}