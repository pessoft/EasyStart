class OrdersPage {
    constructor() {
        this.logic = new OrdersPageLogic()
        this.view = new OrdersPageView()

        this.init()
    }

    async init() {
        this.render()
    }

    render() {
        this.view.render()
    }
}