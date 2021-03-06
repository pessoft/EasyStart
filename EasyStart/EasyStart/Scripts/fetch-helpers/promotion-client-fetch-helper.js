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
}