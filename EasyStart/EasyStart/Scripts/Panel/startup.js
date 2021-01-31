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
        routeConfig[Navigator.pageIds.products] = () => new ProductsPage()
        routeConfig[Navigator.pageIds.promotion] = () => new PromotionPage()
        routeConfig[Navigator.pageIds.users] = () => new UsersPage()
        routeConfig[Navigator.pageIds.settings] = () => new SettingsPage()
        routeConfig[Navigator.pageIds.tariffsAndPay] = () => new TariffsPage()

        this.app.navigator = new Navigator(routeConfig)

    }
    
    hidePreloader() {
        document.querySelector('#preloader-app').style.display = 'none'
    }
}