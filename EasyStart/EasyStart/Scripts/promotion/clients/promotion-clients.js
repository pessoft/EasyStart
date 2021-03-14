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

        this.view.addEventListener(CustomEventListener.events.promotionClients.BLOCK_CLIENT,
            this.blockClientHandler)

        this.view.addEventListener(CustomEventListener.events.promotionClients.UN_BLOCK_CLIENT,
            this.unBlockClientHandler)

        this.view.addEventListener(CustomEventListener.events.promotionClients.SET_CLIENT_VIRTUAL_MONEY,
            this.setClientVirtualMoneyHandler)

        this.view.addEventListener(CustomEventListener.events.promotionClients.SHOW_ORDER_DETAILS,
            this.showOrderDetailsHandler)
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
        this.view.showActivitiIndicatorClientCard()
        const client = this.logic.getClientById(clientId)
        const orders = await this.logic.getOrders(clientId)

        this.view.renderClientInfo(client, orders)
        this.view.hideActivitiIndicatorClientCard()
    }

    blockClientHandler = async ({ clientId }) => {
        await this.toggleBlockClient(clientId, true)
    }

    unBlockClientHandler = async ({ clientId }) => {
        await this.toggleBlockClient(clientId, false)
    }

    toggleBlockClient = async (clientId, blocked) => {
        if (blocked)
            await this.logic.blockClient(clientId)
        else
            await this.logic.unBlockClient(clientId)

        const clients = await this.logic.getClients()

        this.view.render(clients)
        this.view.goToClient(clientId)
    }

    setClientVirtualMoneyHandler = async ({ clientId, virtualMoney }) => {
        this.view.showActivitiIndicatorClientCard()

        await this.logic.setClientVirtualMoney(clientId, virtualMoney)
        const clients = await this.logic.getClients()

        this.view.render(clients)
        this.view.goToClient(clientId)
    }

    showOrderDetailsHandler = ({ order }) => {
        const orderWithUpCasePromNamesJson = JSON.stringify(order, function (key, value) {
            if (value && typeof value === 'object') {
                var replacement = {};
                for (var k in value) {
                    if (Object.hasOwnProperty.call(value, k)) {
                        replacement[k && k.charAt(0).toUpperCase() + k.substring(1)] = value[k];
                    }
                }
                return replacement;
            }
            return value;
        });
        const orderWithUpCasePromNames = JSON.parse(orderWithUpCasePromNamesJson)

        showOrderDetailsFromPromotionClient(orderWithUpCasePromNames)
    }
}