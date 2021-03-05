class PromotionClientsLogic {
    constructor() {
        this.clients = null
        this.orderDictionary = {}
        this.fetch = new PromotionClientFetchHelper()
    }

    async getClients() {
        if (!this.clients)
            this.clients = await this.fetch.getClients()
    }

    async getOrders(clientId) {

    }

    findById(id) {
        if (!this.clients)
            return null

        return this.clients.find(p => p.Id == id)
    }
}