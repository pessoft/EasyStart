﻿class PromotionClientsView extends CustomEventListener {

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
        clientCardContainer: 'promotion-client-card-wrapper',
        blockBtn: 'promotion-client-block-action',
        unBlockBtn:'promotion-client-unblock-action'
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

        this.renderClientInfoDefault()
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
        if (client)
            this.renderClientInfoContent(client, orders)
        else
            this.renderClientInfoDefault()
    }

    renderClientInfoContent(client, orders) {
        this.renderClientCardInfo(client)
        this.renderClientInfoOrders(orders)
    }

    renderClientInfoDefault() {
        const template = `
            <div class="empty-container empty-container-with-icon">
                <i class="fal fa-address-card"></i>
                <span>Карточка клиента</span>
            </div>`

        $(`#${this.storeIds.clientCardContainer}`).html(template)
    }

    renderClientCardInfo(client) {
        const blockToggleTemplate = client.blocked ?
            `<button id="${this.storeIds.unBlockBtn}" class="btn btn-submit">
                <i class="fal fa-lock-open-alt"></i>
                Разблокировать
            </button>` :
            `<button id="${this.storeIds.blockBtn}" class="btn btn-submit">
                <i class="fal fa-lock-alt"></i>
                Заблокировать
            </button>`

        const $template = $(`
            <div class="promotion-client-card-info">
                <h2>${client.userName}</h2>
                <span><i class="fal fa-mobile-alt"></i> ${client.phoneNumber}</span>
                <span><i class="fal fa-at"></i> ${client.email}</span>
                <span><i class="fal fa-key"></i> ${client.password}</span>
                <div>
                    <button type="button" class="simple-text-btn main-bg-color">
                        <i class="fal fa-coins"></i> ${client.virtualMoney} руб.
                    </button>
                </div>
                <div>${blockToggleTemplate}</div>
            </div>
        `)

        if (client.blocked) {
            $template.find(`#${this.storeIds.unBlockBtn}`).click(() => {
                this.dispatchEvent(
                    CustomEventListener.events.promotionClients.UN_BLOCK_CLIENT,
                    {clientId: client.id})
            })
        } else {
            $template.find(`#${this.storeIds.blockBtn}`).click(() => {
                this.dispatchEvent(
                    CustomEventListener.events.promotionClients.BLOCK_CLIENT,
                    { clientId: client.id })
            })
        }

        $(`#${this.storeIds.clientCardContainer}`).html($template)
    }

    renderClientInfoOrders(orders) {
        if (orders && orders.length)
            this.renderClientOrdersContent(orders)
        else
            this.renderClientInfoOrdersEmpty()
    }

    renderClientOrdersContent(orders) {
        const template = `
                <div class="promotion-client-card-orders"
                    
                </div>`

        $(`#${this.storeIds.clientCardContainer}`).append(template)
    }

    renderClientInfoOrdersEmpty() {
        const template = `
                <div class="promotion-client-card-orders">
                    <h2>Заказы клиента</h2>
                    <div class="promotion-client-card-order-list">
                        <div class="empty-container empty-container-with-icon">
                            <i class="fal fa-shopping-basket"></i>
                            <span>Список заказов пуст</span>
                        </div>
                    </div>
                </div>`

        $(`#${this.storeIds.clientCardContainer}`).append(template)
    }

    showActivitiIndicatorPage() {
        this.showActivityIndicator(`#${this.storeIds.clientsPage}`, 'main-bg-color')
    }


    hideActivitiIndicatorPage() {
        this.hideActivityIndicator(`#${this.storeIds.clientsPage}`)
    }

    showActivitiIndicatorClientCard() {
        this.showActivityIndicator(`#${this.storeIds.clientCardContainer}`)
    }


    hideActivitiIndicatorClientCard() {
        this.hideActivityIndicator(`#${this.storeIds.clientCardContainer}`)
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