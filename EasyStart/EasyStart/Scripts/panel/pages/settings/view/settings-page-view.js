class SettingsPageView extends BasePageView {
    constructor() {
        super()
    }

    storeIds = {
        page: Navigator.pageIds.settings
    }

    render() {
        super.render()

        const tabs = this.renderTabs()

        this.insert(`#${this.storeIds.page}`, tabs)
    }

    tabsName = ['Адрес и контакты', 'Доставка', 'Уведомления', 'Интеграция', 'Филиалы']
    renderTabs() {
        let ul = `<ul class="nav nav-pills nav-justified mb-3" id="ex1" role="tablist">`
        for (let i = 0; i < this.tabsName.length; ++i) {
            const tabName = this.tabsName[i]
            const activeClass = i == 0 ? 'active' : ''
            ul += `
                <li class="nav-item" role="presentation">
                    <a
                      class="nav-link ${activeClass}"
                      id="ex3-tab-1"
                      data-mdb-toggle="pill"
                      href="#ex3-pills-1"
                      role="tab"
                      aria-controls="ex3-pills-1"
                      aria-selected="true"
                      >
                        ${tabName}
                    </a>
                </li>`
        }
        ul += '</ul>'

        return ul
    }
}