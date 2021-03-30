class SettingsPageView extends BasePageView {
    constructor() {
        super()

        this.contancsView = new SettingsContactsView()
        this.generalView = new SettingsGeneralView()
        this.orderPaymentView = new SettingsOrderPaymentView()
        this.integrationView = new SettingsIntegrationView()
        this.branchView = new SettingsBranchView()
    }

    storeIds = {
        page: Navigator.pageIds.settings
    }

    render() {
        super.render()
        this.renderContent()
        this.initControls()
    }

    renderContent() {
        const navTabs = this.renderNavTabs()
        const contentTabs = this.renderContentTabs()
        const card = this.renderCard(navTabs, contentTabs)

        this.insert(`#${this.storeIds.page}`, card)
    }


    renderNavTabs() {
        const tabs = [
            this.contancsView,
            this.generalView,
            this.integrationView,
            this.branchView]
        let ul = `<ul class="nav nav-tabs card-header-tabs nav-fill" role="tablist">`
        for (let i = 0; i < tabs.length; ++i) {
            const info = tabs[i].getInfo()
            const activeClass = i == 0 ? 'active' : ''
            ul += `
                <li class="nav-item" role="presentation">
                    <a
                      class="nav-link ${activeClass}"
                      data-mdb-toggle="tab"
                      href="#${info.tabContentId}"
                      role="tab">
                      ${info.tabName}
                    </a>
                </li>`
        }
        ul += '</ul>'

        return ul
    }

    renderContentTabs() {
        const content = []
        content.push(this.contancsView.render())

        return content.join('')
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