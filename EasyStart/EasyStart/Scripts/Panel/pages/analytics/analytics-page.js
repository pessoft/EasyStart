class AnalyticsPage {
    constructor() {
        this.logic = new AnalyticsPageLogic()
        this.view = new AnalyticsPageView()

        this.init()
    }

    async init() {
        this.render()
    }

    render() {
        this.view.render()
    }
}