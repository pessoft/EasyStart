function changeIngredientType(radioId) {
    $(`#${radioId}`).click()
}

const StyleTypeIngredient = {
    Short: 0,
    Long: 1
}

var categoryIngredient = null

function getIngredientData(image) {
    let id = $('#product-ingredient-id').val()

    return {
        id: id && id > 0 ? id : -1,
        ingredientName: $('#name-product-ingredient').val(),
        additionaInfo: $('#product-ingredient-additional-info').val(),
        price: $('#product-ingredient-price').val(),
        minRequiredCount: $('#product-min-ingredient-count').val(),
        maxAddCount: $('#product-ingredient-max-count').val(),
        description: $('#"ingredient-description-product').val(),
        image: image

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
                StyleTypeIngredient: StyleTypeIngredient.Short
            },
            Ingredients: []
        }
        this.categoryIngredient = categoryIngredient != null && Object.keys(categoryIngredient).length > 0 ?
            categoryIngredient :
            defaultCategoryIngredient
        this.tmpURL = []
        this.categoryIngredientOrigin = { ...categoryIngredient }
        const reducer = (acc, value, index) => acc[value.Id] = index
        this.indexIngredients = this.categoryIngredient.Ingredients.reduce(reducer)
    }

    addOrUpdateIngredient(ingredient) {
        if (ingredient.id > 0) {
            if (this.indexIngredients[ingredient.id]) {
                const index = this.indexIngredients[ingredient.id]
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
                } else {
                    showErrorMessage(result.ErrorMessage);
                }
            }

            $.post("/Admin/AddCategoryConstructor", category, successCallBack(successFunc, loader));
        }
    }

    setData() {

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