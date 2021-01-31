class BasePageView {
    storeIds = {
        page: ''
    }
    pageCssClass = 'partial-page'
    isPreparedPage = false

    preparePage() {
        if(this.isPreparedPage)
            return

        this.hideAllPages()
        this.showPage()

        this.isPreparedPage = true
    }

    hideAllPages() {
        const pages = document.querySelectorAll(`.${this.pageCssClass}`)
        pages.forEach(p => p.style.display = 'none')
    }

    showPage() {
        const page = document.querySelector(`#${this.storeIds.page}`)
        
        if(page)
            page.style.display = 'grid'
    }

    render() {
        this.preparePage()
    }
}