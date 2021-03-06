class PromotionClientsView extends CustomEventListener {

    constructor() {
        super()

        this.clientPage = {
            currentNumber: 0,
            countPages: 0,
            maxCountItemsInPage: 30,
        }
        this.clients = []
        this.prefix = 'promotion-client'
    }

    storeIds = {
        clientsPage: 'promotion-clients',
        clientList: 'promotions-client-list',
        pagination: 'promotion-clients-pagination',
        inputPhoneNumber: 'promotion-client-phone-number',
        goToClientBtn: 'promotion-find-client-action',
    }

    storeCssClass = {
        previusPageBtn: 'promotion-previus-clients-page',
        nextPageBtn: 'promotion-next-clients-page',
        activeClient: 'promotion-active-client',
    }

    render(clients) {
        this.renderFindByPhoneBlock()

        if (clients && clients.length) {
            this.clients = clients

            this.initPagesSetting()
            this.renderClientsPage()
        }
        else
            this.renderEmptyClientItems()
    }

    renderFindByPhoneBlock() {
        this.renderInputPhoneNumber()
        this.renerGoToBtn()
    }

    renderInputPhoneNumber() {
        const self = this
        const mask = '+7(999)999-99-99'
        const $inputPhoneNumber = $(`#${this.storeIds.inputPhoneNumber}`)
        $inputPhoneNumber.val('')

        $inputPhoneNumber.mask(mask, {
            completed: function () {
                self.dispatchEvent(
                    CustomEventListener.events.promotionClients.FIND_BY_PHONE,
                    { phoneNumber: $(this).val() }
                )
            }
        })

        $inputPhoneNumber.on('keydown', function () {
            if (!$(this).val().match(/^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$/))
                self.disabledGoToClientBtn()
        })
    }

    renerGoToBtn() {
        this.disabledGoToClientBtn()
        this.bindGoToEvent()
    }

    bindGoToEvent() {
        $(`#${this.storeIds.goToClientBtn}`).click(() => {
            const phoneNumber = $(`#${this.storeIds.inputPhoneNumber}`).val()

            this.dispatchEvent(
                CustomEventListener.events.promotionClients.GO_TO_CLIENT,
                { phoneNumber }
            )
        })
    }

    disabledGoToClientBtn() {
        $(`#${this.storeIds.goToClientBtn}`).prop('disabled', true)
    }

    enebledGoToClientBtn() {
        $(`#${this.storeIds.goToClientBtn}`).prop('disabled', false)
    }

    goToClient(clientId) {
        const index = this.clients.findIndex(p => p.id == clientId)
        const targetPageNumber = parseInt(index / this.clientPage.maxCountItemsInPage) + 1

        if (this.clientPage.currentNumber != targetPageNumber)
            this.renderClientsPage(targetPageNumber)

        this.activeClient(clientId)
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

    generateClientItemId(clientId) {
        return `${this.prefix}${clientId}`
    }

    getClientItem(client) {
        const $template = $(`
            <div class="promotion-client-item" id="${this.generateClientItemId(client.id)}">
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
            </div>`)

        $template.click(() => this.activeClient(client.id))

        return $template
    }

    renderPagination() {
        const previusDisabled = this.clientPage.currentNumber <= 1 ? 'disabled' : ''
        const nextDisabled = this.clientPage.currentNumber == this.clientPage.countPages ? 'disabled' : ''

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

    markActiveClient(clientId) {
        const id = this.generateClientItemId(clientId)

        $(`#${this.storeIds.clientList} .${this.storeCssClass.activeClient}`).removeClass(this.storeCssClass.activeClient)
        $(`#${id}`).addClass(this.storeCssClass.activeClient)
    }

    activeClient(clientId) {
        this.markActiveClient(clientId)
        this.onActiveClient(clientId)
    }

    onActiveClient(clientId) {
        this.dispatchEvent(
            CustomEventListener.events.promotionClients.ACTIVE_CLIENT,
            { clientId }
        )
    }

    renderClientInfo(client, orders) {

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
    showActivityIndicator(query, customClass = '') {
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