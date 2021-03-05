class PromotionClients {
    constructor() {
        this.view = new PromotionClientsView()
        this.logic = new PromotionClientsLogic()

        this.init()
    }

    async init() {
        try {
            this.view.showActivitiIndicatorPage()
            const clients = await this.logic.getClients()

            this.view.render(clients)
        } catch (ex) {
            console.error(ex)
        } finally {
            this.view.hideActivitiIndicatorPage()
        }
    }
}