class Startup {
    constructor() {
        this.app = {}
    }

    run() {

    }

    async configure() {
        this.initNavigator()
    }

    initNavigator() {
        const routeConfig = {}
        this.app.navigator = new Navigator(routeConfig)

    }
    
}