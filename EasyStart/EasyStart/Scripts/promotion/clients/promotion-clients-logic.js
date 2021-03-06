class PromotionClientsLogic {
    constructor() {
        this.clients = null
        this.clientsDictByPhone = {}
        this.fetch = new PromotionClientFetchHelper()
    }

    async getClients() {
        if (!this.clients) {
            this.clients = await this.fetch.getClients()
            this.clients.forEach(client => this.clientsDictByPhone[client.phoneNumber] = client)
        }

        return this.clients
    }

    async getOrders(clientId) {
        return await this.fetch.getClientOrders(clientId)
    }

    getClientById(id) {
        if (!this.clients)
            return null

        return this.clients.find(p => p.Id == id)
    }

    getClientByPhoneNumber(phoneNumber) {
        return this.clientsDictByPhone[phoneNumber]
    }
}