class PromotionClientsView extends CustomEventListener {

    constructor() {
        super()

        this.clientPage = {
            currentNumber: 0,
            countPages: 0,
            maxCountItemsInPage: 30,
        }
        this.clients = []
    }

    storeIds = {
        clientsPage: 'promotion-clients',
        clientList: 'promotions-client-list',
        pagination: 'promotion-clients-pagination'
    }

    storeCssClass = {
        previusPageBtn: 'promotion-previus-clients-page',
        nextPageBtn: 'promotion-next-clients-page',
    }

    render(clients) {
        
        if (clients && clients.length) {
            this.clients = clients

            this.initPagesSetting()
            this.renderClientsPage()
        }
        else
            this.renderEmptyClientItems()
    }

    renderClientsPage(pageNumber = 1) {
        this.clientPage.currentNumber = pageNumber

        this.renderClinetItems(pageNumber)
        this.renderPagination()
    }

    initPagesSetting() {
        if (this.clients.length) {
            let countPage = 0

            if (this.clients.length <= this.clientPage.maxCountItemsInPage)
                countPage = 1
            else {
                countPage = this.clients.length / this.clientPage.maxCountItemsInPage

                if (countPage * this.clientPage.maxCountItemsInPage != this.clients.length)
                    ++countPage
            }

            this.clientPage.countPages = countPage
        }
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

    renderClinetItems(pageNumber = 1) {
        const items = []

        let endIndex = pageNumber * this.clientPage.maxCountItemsInPage
        let startIndex = endIndex - this.clientPage.maxCountItemsInPage

        if (endIndex > this.clients.length) {
            endIndex = this.clients.length
            startIndex = 0
        }

        for (let i = startIndex; i < endIndex; ++i) {
            const item = this.getClientItem(this.clients[i])
            items.push(item)
        }

        $(`#${this.storeIds.clientList}`).html(items)
    }

    getClientItem(client) {
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
        const previusDisabled = this.clientPage.currentNumber <= 1 ? 'disabled' : ''
        const nextDisabled = this.clientPage.currentNumber  ==  this.clientPage.countPages ? 'disabled' : ''

        const $template = $(`
            <button ${previusDisabled} class="simple-text-btn main-bg-color ${this.storeCssClass.previusPageBtn}">
                    « Назад
            </button>
            <button ${nextDisabled} class="simple-text-btn main-bg-color ${this.storeCssClass.nextPageBtn}">
                Далее »
            </button>
        `)

        const $paginationContainer = $(`#${this.storeIds.pagination}`)
        $paginationContainer.html($template)

        $paginationContainer.find(`.${this.storeCssClass.previusPageBtn}`).click(() => {
            this.dispatchEvent(CustomEventListener.events.promotionClients.CHANGE_CLIENTS_PAGE,
                { pageNumber: this.clientPage.currentNumber - 1 })
        })
        $paginationContainer.find(`.${this.storeCssClass.nextPageBtn}`).click(() => {
            this.dispatchEvent(CustomEventListener.events.promotionClients.CHANGE_CLIENTS_PAGE,
                { pageNumber: this.clientPage.currentNumber + 1 })
        })
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