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
        Dialog.clear('#addProducIngredientDialog')
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

function removeIngredientById(id, event) {
    event.stopPropagation()

    categoryIngredient.removeIngredientById(id)
}

function removeIngredientByUniqId(uniqId, event) {
    event.stopPropagation()

    categoryIngredient.removeIngredientByUniqId(uniqId)
}

function showIngredientDataById(id, event) {
    event.stopPropagation()

    categoryIngredient.setIngredientDialogDataById(id)
    Dialog.showModal('#addProducIngredientDialog')
}

function showIngredientDataByUniqId(uniqId, event) {
    event.stopPropagation()

    categoryIngredient.setIngredientDialogDataByUniqId(uniqId)
    Dialog.showModal('#addProducIngredientDialog')
}

function cancelChange() {
    categoryIngredient.revert()
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
        this.categoryIngredientOrigin = cloneObject(this.categoryIngredient)
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
            const index = this.categoryIngredient.Ingredients.findIndex(p => p.TMPUniqId == ingredient.TMPUniqId)

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
                    this.categoryIngredientOrigin = cloneObject(result.Data)
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
        this.clearIngredientRows()
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
        let attrId = ingredient.Id > 0 ? `ingredient-id=${ingredient.Id}` : `ingredient-uniqId=${ingredient.TMPUniqId}`
        let removeAction = ingredient.Id > 0 ? `removeIngredientById(${ingredient.Id}, event)` : `removeIngredientByUniqId('${ingredient.TMPUniqId}', event)`
        let editAction = ingredient.Id > 0 ? `showIngredientDataById(${ingredient.Id}, event)` : `showIngredientDataByUniqId('${ingredient.TMPUniqId}', event)`

        return `
        <div class="ingredient-row" ${attrId}>
            <img src="${ingredient.Image}" alt="ingredient image" />
            <div class="product-ingredient-wrapper">
                <span class="ingredient-row-name">${ingredient.Name}</span>
                <span class="ingredient-row-small-text">${ingredient.AdditionaInfo}</span>
                <span class="ingredient-row-small-text">${ingredient.Price} руб.</span>
                <div class="ingredient-row-menu">
                    <i class="fal fa-edit" onclick="${editAction}"></i>
                    <i class="fal fa-trash-alt" onclick="${removeAction}"></i>
                </div>
            </div>
        </div>
        `
    }

    removeIngredientById(id) {
        if (id > 0) {
            const index = this.indexIngredients[id]
            const self = this

            $(`[ingredient-id=${id}]`).fadeOut(250, function () {
                $(this.remove)
                self.removeIngredient(index)
            })

        }
    }

    removeIngredientByUniqId(uniqId) {
        const findIndex = this.categoryIngredient.Ingredients.findIndex(p => p.TMPUniqId == uniqId)
        const self = this

        if (findIndex >= 0) {
            $(`[ingredient-uniqId='${uniqId}']`).fadeOut(250, function () {
                $(this).remove()
                self.removeIngredient(findIndex)
            })
        }
    }

    removeIngredient(index) {
        this.categoryIngredient.Ingredients.splice(index, 1)

        if (this.categoryIngredient.Ingredients.length == 0) {
            this.setEmptyList()
        }
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

    setIngredientDialogDataById(id) {
        if (id > 0) {
            const index = this.indexIngredients[id]
            const ingredient = this.categoryIngredient.Ingredients[index]

            this.setIngredientDialogData(ingredient)
        }
    }

    setIngredientDialogDataByUniqId(uniqId) {
        const findIndex = this.categoryIngredient.Ingredients.findIndex(p => p.TMPUniqId == uniqId)
        const ingredient = this.categoryIngredient.Ingredients[findIndex]

        if (findIndex >= 0) {
            this.setIngredientDialogData(ingredient)
        }
    }

    setIngredientDialogData(ingredient) {
        $('#product-ingredient-id').val(ingredient.Id)
        $('#name-product-ingredient').val(ingredient.Name)
        $('#product-ingredient-additional-info').val(ingredient.AdditionaInfo)
        $('#product-ingredient-price').val(ingredient.Price)
        $('#product-min-ingredient-count').val(ingredient.MinRequiredCount)
        $('#product-ingredient-max-count').val(ingredient.MaxAddCount)
        $('#ingredient-description-product').val(ingredient.Description)

        $("#addProducIngredientDialog img").attr("src", ingredient.Image)
        $("#addProducIngredientDialog img").removeClass('hide')
        $("#addProducIngredientDialog .dialog-image-upload").addClass('hide')

        $('#product-ingredient-tmp-uniqId').val(ingredient.TMPUniqId)
    }

}