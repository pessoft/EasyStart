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

        this.view.addEventListener(CustomEventListener.events.promotionClients.FIND_BY_PHONE,
            this.findByPhoneNumberHandler)

        this.view.addEventListener(CustomEventListener.events.promotionClients.GO_TO_CLIENT,
            this.goToClientHandler)

        this.view.addEventListener(CustomEventListener.events.promotionClients.ACTIVE_CLIENT,
            this.activeClientHandler)
    }

    changeClientPageHandler = ({ pageNumber }) => {
        this.view.renderClientsPage(pageNumber)
    }

    findByPhoneNumberHandler = ({ phoneNumber }) => {
        const client = this.logic.getClientByPhoneNumber(phoneNumber)

        if (client)
            this.view.enebledGoToClientBtn()
        else
            this.view.disabledGoToClientBtn()
    }

    goToClientHandler = ({ phoneNumber }) => {
        const client = this.logic.getClientByPhoneNumber(phoneNumber)
        this.view.goToClient(client.id)
        this.view.renderFindByPhoneBlock()
    }

    activeClientHandler = async ({ clientId }) => {
        const client = this.logic.getClientById(clientId)
        const orders = await this.logic.getOrders(clientId)

        this.view.renderClientInfo(client, orders)
    }
}