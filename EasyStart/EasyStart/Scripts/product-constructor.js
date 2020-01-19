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
        AdditionalInfo: $('#product-ingredient-additional-info').val(),
        Price: $('#product-ingredient-price').val(),
        MaxAddCount: $('#product-ingredient-max-count').val(),
        Description: $('#ingredient-description-product').val(),
        Image: image,
        TMPUniqId: $('#product-ingredient-tmp-uniqId').val(),
        IsDeleted: false

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

        categoryIngredient.addOrUpdateIngredient(ingredient)
        categoryIngredient.setIngredients()

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

function updateGloabalDataProduct(productConstructor) {
    const index = DataProduct.Products.findIndex(p => p.Id == productConstructor.Id)

    if (index >= 0)
        DataProduct.Products[index] = productConstructor
    else {
        DataProduct.Products.push(productConstructor)
        maybeClearProductList()
        addProductConstructorToList(productConstructor)
    }
        
}

function maybeClearProductList() {
    if ($(".product-list").find('.empty-list').length > 0) {
        clearProductList()
    }
}

function fixMinMaxCount() {
    const min = $('#category-constructor-min-count').val()
    const max = $('#category-constructor-max-count').val()

    if (min > max) {
        $('#category-constructor-min-count').val(max)
    }
}

function saveCategoruConstructor() {
    fixMinMaxCount()
    categoryIngredient.save(updateGloabalDataProduct)
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
    categoryIngredient.setData()
}

function getCategoryConstructorById(id) {
    const constructors = DataProduct.Products.filter(p => p.Id == id)

    return constructors && constructors.length > 0 ? constructors[0] : null
}

function editCategoryConstructor(id, event) {
    event.stopPropagation()

    var categoryConstructor = getCategoryConstructorById(id)

    if (categoryConstructor) {
        categoryIngredient = new CategoryIngredient(cloneObject(categoryConstructor))

        Dialog.clear('#addSubCategoryConstructorDialog')
        categoryIngredient.setData()
        Dialog.showModal('#addSubCategoryConstructorDialog')
    }
    else {
        showErrorMessage('Категория конструктора не найдена')
    }
}

function removeCategoryConstructor(id) {
    event.stopPropagation();

    let callback = function () {
        $(`[product-id=${id}]`).fadeOut(500, function () {
            $(this).remove();

            if ($(".product-list").children().length == 0) {
                setEmptyProductInfo();
            }
        });

        $.post("/Admin/RemoveCategoryConstructor", { categoryConstructorId: id });
    }

    deleteConfirmation(callback);
}

function removeIngredientById(id, event) {
    event.stopPropagation()

    let callback = function () {
        categoryIngredient.removeIngredientById(id)
    }

    deleteConfirmation(callback);
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
            Id: -1,
            CategoryId: SelectIdCategoryId,
            Name: '',
            MinCountIngredient: 0,
            MaxCountIngredient: 0,
            StyleTypeIngredient: StyleTypeIngredient.Short,
            OrderNumber: DataProduct.Products ? DataProduct.Products.length + 1 : 1,
            Ingredients: []
        }
        this.categoryIngredient = categoryIngredient && Object.keys(categoryIngredient).length > 0 ?
            categoryIngredient :
            defaultCategoryIngredient
        this.categoryIngredientOrigin = cloneObject(this.categoryIngredient)
        const reducer = function (acc, value, index) { acc[value.Id] = index; return  acc}
        this.indexIngredients = this.categoryIngredient.Ingredients.length > 0 ?
            this.categoryIngredient.Ingredients.reduce(reducer, {}) :
            {}
    }

    addOrUpdateIngredient(ingredient) {
        if (ingredient.Id > 0) {
            if (this.indexIngredients[ingredient.Id] >= 0) {
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

    isNotEmptyIngredients() {
        let result = false

        if (this.categoryIngredient.Ingredients
            && this.categoryIngredient.Ingredients.length > 0
            && this.categoryIngredient.Ingredients.filter(p => !p.IsDeleted).length > 0) {
            result = true;
        }

        return result
    }

    save(updateGloabalDataProductFunc) {
        if (this.isNotEmptyIngredients()) {

            let loader = new Loader($("#addSubCategoryConstructorDialog form"));
            loader.start();
            const self = this

            let constructor = {
                Id: this.categoryIngredient.Id,
                CategoryId: this.categoryIngredient.CategoryId,
                Name: $('#category-constructor-name').val(),
                MinCountIngredient: $('#category-constructor-min-count').val(),
                MaxCountIngredient: $('#category-constructor-max-count').val(),
                StyleTypeIngredient: $('#ingredient-view-type-short').is(':checked') ? StyleTypeIngredient.Short : StyleTypeIngredient.Long,
                Ingredients: this.categoryIngredient.Ingredients
            }

            let successFunc = function (result, loader) {
                loader.stop();
                if (result.Success) {
                    if (updateGloabalDataProductFunc) {
                        updateGloabalDataProductFunc(cloneObject(result.Data))
                    }

                    self.categoryIngredient = result.Data
                    self.categoryIngredientOrigin = cloneObject(result.Data)

                    self.clearIngredientRows()
                    self.setData()
                } else {
                    showErrorMessage(result.ErrorMessage);
                }
            }

            $.post("/Admin/AddOrUpdateCategoryConstructor", constructor, successCallBack(successFunc, loader));
        } else {
            showInfoMessage('Добавте ингредиенты')
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
        this.setConstructorCategoryParams()
        this.setIngredients()
        
    }

    setIngredients() {
        this.removeEmptyList()
        this.clearIngredientRows()
        this.setIngredientList()
    }

    setConstructorCategoryParams() {
        $('#category-constructor-name').val(this.categoryIngredient.Name)
        $('#category-constructor-min-count').val(this.categoryIngredient.MinCountIngredient)
        $('#category-constructor-max-count').val(this.categoryIngredient.MaxCountIngredient)

        switch (this.categoryIngredient.StyleTypeIngredient) {
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
            if (!ingredient.IsDeleted) {
                ingredientRows.push(this.renderIngredientRow(ingredient))
            }
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
                <span class="ingredient-row-small-text">${ingredient.AdditionalInfo}</span>
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
                $(this).remove()
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

    removeIngredient(index, isRemove = false) {
        if (isRemove) {
            this.categoryIngredient.Ingredients.splice(index, 1)
        } else {
            this.categoryIngredient.Ingredients[index].IsDeleted = true
        }
        

        if (this.categoryIngredient.Ingredients.filter(p => !p.IsDeleted).length == 0) {
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
        this.categoryIngredient = cloneObject(this.categoryIngredientOrigin)
        this.setData()
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
        $('#product-ingredient-additional-info').val(ingredient.AdditionalInfo)
        $('#product-ingredient-price').val(ingredient.Price)
        $('#product-ingredient-max-count').val(ingredient.MaxAddCount)
        $('#ingredient-description-product').val(ingredient.Description)

        $("#addProducIngredientDialog img").attr("src", ingredient.Image)
        $("#addProducIngredientDialog img").removeClass('hide')
        $("#addProducIngredientDialog .dialog-image-upload").addClass('hide')

        $('#product-ingredient-tmp-uniqId').val(ingredient.TMPUniqId)
    }
}