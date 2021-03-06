class PromotionClients {
    constructor() {
        this.view = new PromotionClientsView()
        this.logic = new PromotionClientsLogic()

        this.bindEvents()
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

    bindEvents() {
        this.view.addEventListener(CustomEventListener.events.promotionClients.CHANGE_CLIENTS_PAGE,
            this.changeClientPageHandler)
    }

    changeClientPageHandler = ({ pageNumber }) => {
        this.view.renderClientsPage(pageNumber)
    }
}