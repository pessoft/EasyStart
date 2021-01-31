class Startup {
    constructor() {
        this.app = {}
    }

    run() {
        this.app.navigator.initActivePage()
        this.hidePreloader()
    }

    async configure() {
        this.initNavigator()
    }

    initNavigator() {
        const routeConfig = {}
        routeConfig[Navigator.pageIds.orders] = () => new OrdersPage()
        routeConfig[Navigator.pageIds.ordersHistory] = () => new OrdersHistoryPage()

        this.app.navigator = new Navigator(routeConfig)

    }
    
    hidePreloader() {
        document.querySelector('#preloader-app').style.display = 'none'
    }
}