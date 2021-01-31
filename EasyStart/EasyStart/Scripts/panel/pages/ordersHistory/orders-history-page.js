class OrdersHistoryPage {
    constructor() {
        this.logic = new OrdersHistoryPageLogic()
        this.view = new OrdersHistoryPageView()

        this.init()
    }

    async init() {
        this.render()
    }

    render() {
        this.view.render()
    }
}