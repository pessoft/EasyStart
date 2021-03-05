class PromotionClientsView extends CustomEventListener {

    constructor() {
        super()
    }

    storeIds = {
        clientsPage: 'promotion-clients',
        clientList: 'promotions-client-list',
    }

    render(clients) {
        if (clients && clients.length)
            this.renderClinetItems(clients)
        else
            this.renderEmptyClientItems()
    }

    renderEmptyClientItems() {
        const template = `
            <div class="empty-container empty-container-with-icon">
                <i class="fal fa-users"></i>
                <span>Список клиентов</span>
            </div>
        `

        $(`#${this.storeIds.clientList}`).html(template)
    }

    renderClinetItems(clients) {
        const items = []

        for (const client of clients) {
            const item = this.renderClientItem(client)
            items.push(item)
        }

        $(`#${this.storeIds.clientList}`).html(items)
    }

    renderClientItem(client) {
        const template = `
            <div class="promotion-client-item">
                <div class="promotion-client-item-short-info">
                    <span class="promotion-client-item-phoneNumber">
                        <i class="fal fa-mobile-alt"></i>
                        ${client.phoneNumber}
                    </span>
                    <span class="promotion-client-item-name">
                        <i class="fal fa-user"></i>
                        ${client.userName}
                     </span>
                </div>
                <div class="promotion-client-item-blocked-info">
                    ${client.blocked ? '<i class="fal fa-lock-alt"></i>' : '<i class="fal fa-lock-open-alt"></i>'}
                </div>
            </div>`

        return template
    }

    renderPagination() {

    }

    renderClientdInfo() {

    }

    showActivitiIndicatorPage() {
        this.showActivityIndicator(`#${this.storeIds.clientsPage}`, 'main-bg-color')
    }


    hideActivitiIndicatorPage() {
        this.hideActivityIndicator(`#${this.storeIds.clientsPage}`)
    }

    /**
     * 
     * @private
     */
    showActivityIndicator(query, customClass='') {
        const loader = new Loader(query)
        loader.start(customClass)
    }

    /**
     *
     * @private
     */
    hideActivityIndicator(query) {
        const loader = new Loader(query)
        loader.stop()
    }
}