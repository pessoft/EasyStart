class Navigator {
    /**
     * 
     * @param {object} routeConfig { key: pageId, value: function}
     * @param {string} defaultActivePage - default orders page
     */
    constructor(routeConfig, defaultActivePage) {
        if (!routeConfig)
            throw new Error(MessageStore.navigator.routeConfigIsRequired)

        this.id = 'navigator'
        this.navItemQuery = `#${this.id} a.nav-link, #${this.id} a.navbar-brand`
        this.mainNavBarQuery = `#${this.id} .navbar-collapse`
        this.navBarTogglerQuery = `#${this.id} .navbar-toggler`
        this.routeConfig = routeConfig
        this.currentPage = defaultActivePage || Navigator.pageIds.orders 

        this.bindEventChangeActiveItem()
    }

    initActivePage() {
        this.changeActive(this.currentPage)
    }

    bindEventChangeActiveItem() {
        const navigatorItems = document.querySelectorAll(this.navItemQuery)
        

        navigatorItems.forEach(p => p.addEventListener('click', event => {
            this.hideCollapseMenuHandler();
            this.changeActiveHandler(event)
        }))
    }

    hideCollapseMenuHandler = () => {
        const navBarToggler = document.querySelector(this.navBarTogglerQuery)
        const style = window.getComputedStyle(navBarToggler)
        if (style.display != 'none') {
            const mainNavBarCollapse = new mdb.Collapse(document.querySelector(this.mainNavBarQuery), { toggle: false })
            mainNavBarCollapse.toggle()
        }
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
        const navigatorItems = document.querySelectorAll(this.navItemQuery)
        navigatorItems.forEach(p => p.classList.remove(activeCssClass))

        const activeItemQuery = `#${this.id} a[target-page='${pageId}']`
        const itemForActive = document.querySelector(activeItemQuery)
        itemForActive.classList.add(activeCssClass)
    }

    changeActivePage(pageId) {
        const rout = this.routeConfig[pageId]
        rout && rout()
    }
}

Navigator.pageIds = {
    aboutUs: 'aboutUs',
    orders: 'orders',
    ordersHistory: 'ordersHistory',
    products: 'products',
    promotion: 'promotion',
    analytics: 'analytics',
    settings: 'settings',
    tariffsAndPay: 'tariffsAndPay'
}