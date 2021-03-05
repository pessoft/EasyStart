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
}