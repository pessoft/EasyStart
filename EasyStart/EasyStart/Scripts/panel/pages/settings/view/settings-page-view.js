class SettingsPageView extends BasePageView {
    constructor() {
        super()
    }

    storeIds = {
        page: Navigator.pageIds.settings
    }

    render() {
        super.render()
        this.renderContent()
    }

    renderContent() {
        const navTabs = this.renderNavTabs()
        const contentTabs = this.renderContentTabs()
        const card = this.renderCard(navTabs, contentTabs)

        this.insert(`#${this.storeIds.page}`, card)
    }


    tabsName = ['Адрес и контакты', 'Доставка', 'Уведомления', 'Интеграция', 'Филиалы']
    renderNavTabs() {
        let ul = `<ul class="nav nav-tabs card-header-tabs nav-fill" role="tablist">`
        for (let i = 0; i < this.tabsName.length; ++i) {
            const tabName = this.tabsName[i]
            const activeClass = i == 0 ? 'active' : ''
            ul += `
                <li class="nav-item" role="presentation">
                    <a
                      class="nav-link ${activeClass}"
                      data-mdb-toggle="tab"
                      href="#"
                      role="tab">
                      ${tabName}
                    </a>
                </li>`
        }
        ul += '</ul>'

        return ul
    }

    renderContentTabs() {
        return ``
    }

    renderCard(navTabs, contentTabs) {
        const template = `
            <div class="card">
                <div class="card-header">
                    ${navTabs}
                </div>
                <div class="card-body">
                ${contentTabs}
                </div>
            </div>`

        return template
    }
}