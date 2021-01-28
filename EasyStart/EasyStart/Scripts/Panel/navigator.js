class Navigator {
    /**
     * 
     * @param {object} routeConfig { key: pageId, value: Page}
     * @param {string} defaultActivePage - default orders page
     */
    constructor(routeConfig, defaultActivePage) {
        if (!routeConfig)
            throw new Error(MessageStore.navigator.routeConfigIsRequired)

        this.id = 'navigator'
        this.query = `#${this.id} a.nav-link, #${this.id} a.navbar-brand`
        this.routeConfig = routeConfig
        this.currentPage = defaultActivePage || Navigator.pageIds.orders 

        this.bindEventChangeActiveItem()
        this.changeActive(this.currentPage)
    }

    bindEventChangeActiveItem() {
        const navigatorItems = document.querySelectorAll(this.query)
        navigatorItems.forEach(p => p.addEventListener('click', this.changeActiveHandler))
    }

    changeActiveHandler = event => {
        const pageId = this.getTargetPageFromElemet(event.currentTarget)

        if (pageId == this.currentPage)
            return

        this.changeActive(pageId)
    }

    changeActive(pageId) {
        this.changeActiveItem(pageId)
        this.changeActivePage(pageId)

        this.currentPage = pageId
    }
    
    getTargetPageFromElemet = element => element.getAttribute('target-page')

    changeActiveItem(pageId) {
        const activeCssClass = 'active'
        const navigatorItems = document.querySelectorAll(this.query)
        navigatorItems.forEach(p => p.classList.remove(activeCssClass))

        const activeItemQuery = `#${this.id} a[target-page='${pageId}']`
        const itemForActive = document.querySelector(activeItemQuery)
        itemForActive.classList.add(activeCssClass)
    }

    changeActivePage(pageId) {
        const page = this.routeConfig[pageId]
        page && page.init()
    }
}

Navigator.pageIds = {
    aboutUs: 'aboutUs',
    orders: 'orders',
    ordersHistory: 'ordersHistory',
    products: 'products',
    promotion: 'promotion',
    users: 'users',
    settings: 'settings',
    tariffsAndPay: 'tariffsAndPay'
}