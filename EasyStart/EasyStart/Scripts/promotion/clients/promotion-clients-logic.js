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

    async blockClient(clientId) {
        await this.fetch.blockClient(clientId)

        this.toggleBlockClient(clientId, true)
    }

    async unBlockClient(clientId) {
        await this.fetch.unBlockClient(clientId)

        this.toggleBlockClient(clientId, false)
    }

    getClientById(id) {
        if (!this.clients)
            return null

        return this.clients.find(p => p.id == id)
    }

    getClientByPhoneNumber(phoneNumber) {
        return this.clientsDictByPhone[phoneNumber]
    }

    /**
     * 
     * @private
     */
    toggleBlockClient(clientId, blocked) {
        const client = this.getClientById(clientId)
        client.blocked = blocked
    }
}