class PromotionPage {
    constructor() {
        this.logic = new PromotionPageLogic()
        this.view = new PromotionPageView()

        this.init()
    }

    async init() {
        this.render()
    }

    render() {
        this.view.render()
    }
}