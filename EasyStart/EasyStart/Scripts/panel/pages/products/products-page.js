class ProductsPage {
    constructor() {
        this.logic = new ProductsPageLogic()
        this.view = new ProductsPageView()

        this.init()
    }

    async init() {
        this.render()
    }

    render() {
        this.view.render()
    }
}