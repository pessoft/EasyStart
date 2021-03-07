class PromotionClientFetchHelper extends BaseFetchHelper {
    constructor() {
        super()
    }

    async getClients() {
        const props = {
            url: UrlStore.getPromotionClients,
            method: FetchHelper.method.GET
        }

        return await this.fetch({}, props)
    }

    async getClientOrders(clientId) {
        const props = {
            url: UrlStore.getClientOrders,
            method: FetchHelper.method.POST
        }

        return await this.fetch({ clientId }, props)
    }

    async blockClient(clientId) {
        const props = {
            url: UrlStore.promotionClientBlock,
            method: FetchHelper.method.POST
        }

        return await this.fetch({ clientId }, props)
    }

    async unBlockClient(clientId) {
        const props = {
            url: UrlStore.promotionClientUnBlock,
            method: FetchHelper.method.POST
        }

        return await this.fetch({ clientId }, props)
    }

    async setVirtualMoney(clientId, virtualMoney) {
        const props = {
            url: UrlStore.promotionSetVirtualMoney,
            method: FetchHelper.method.POST
        }

        return await this.fetch({ clientId, virtualMoney }, props)
    }
}