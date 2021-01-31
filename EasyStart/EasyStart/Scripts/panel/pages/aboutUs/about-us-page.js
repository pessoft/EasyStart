class AboutUsPage {
    constructor() {
        this.logic = new AboutUsPageLogic()
        this.view = new AboutUsPageView()

        this.init()
    }

    async init() {
        this.render()
    }

    render() {
        this.view.render()
    }
}