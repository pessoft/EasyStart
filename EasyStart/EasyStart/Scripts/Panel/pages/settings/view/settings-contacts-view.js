class SettingsContactsView {

    tabName = 'Адрес и контакты'
    tabContentId = 'settings-tabs-contacts'

    getInfo() {
        return {
            tabName: this.tabName,
            tabContentId: this.tabContentId
        }
    }

    render() {
        const address = this.renderAddress()
        const contacts = this.renderContacts()
        const socialNetworks = this.renerSocialNetworks()
        const menu = this.renderMenu()
        const content = this.renderContent([address, contacts, socialNetworks, menu])

        return content
    }

    renderAddress() {
        const template = `
            <div class="row">
                <h5>Адрес</h5>  
                <div class="col-md-4  mb-4">
                    <div class="form-outline">
                      <input type="text" id="address-city" class="form-control" />
                      <label class="form-label fs-7" for="address-city">Город</label>
                    </div>
                </div>
                <div class="col-md-4  mb-4">
                    <div class="form-outline">
                      <input type="text" id="address-street" class="form-control" />
                      <label class="form-label fs-7" for="address-street">Улица</label>
                    </div>
                </div>
                <div class="col-md-4 mb-4">
                    <div class="form-outline">
                      <input type="text" id="address-house-number" class="form-control" />
                      <label class="form-label fs-7" for="address-house-number">Номер дома(офиса)</label>
                    </div>
                </div>
            </div>
        `
        return template
    }

    renderContacts() {
        const template = `
            <div class="row">
                <h5>Контакты</h5>
                <div class="col-md-4 mb-4">
                    <div class="form-outline">
                      <input type="tel" id="contacts-phone-number-primary" class="form-control" />
                      <label class="form-label fs-7" for="contacts-phone-number-primary">Телефон 1</label>
                    </div>
                </div>
                <div class="col-md-4 mb-4">
                    <div class="form-outline">
                      <input type="tel" id="contacts-phone-number-secondary" class="form-control" />
                      <label class="form-label fs-7" for="contacts-phone-number-secondary">Телефон 2</label>
                    </div>
                </div>
                <div class="col-md-4 mb-4">
                    <div class="form-outline">
                      <input type="email" id="contacts-email" class="form-control" />
                      <label class="form-label fs-7" for="contacts-email">Адрес эл.почты</label>
                    </div>
                </div>
            </div>
        `
        return template
    }

    renerSocialNetworks() {
        const template = `
            <div class="row">
                <h5>Соц.сети</h5>
                <div class="col-md-4 mb-4">
                    <div class="form-outline">
                      <input type="url" id="contacts-social-vk" class="form-control" />
                      <label class="form-label fs-7" for="contacts-social-vk">ВКонтакте</label>
                    </div>
                </div>
                <div class="col-md-4 mb-4">
                    <div class="form-outline">
                      <input type="url" id="contacts-social-instagram" class="form-control" />
                      <label class="form-label fs-7" for="contacts-social-instagram">Инстраграм</label>
                    </div>
                </div>
                <div class="col-md-4 mb-4">
                    <div class="form-outline">
                      <input type="url" id="contacts-social-facebook" class="form-control" />
                      <label class="form-label fs-7" for="contacts-social-facebook">Фейсбук</label>
                    </div>
                </div>
            </div>
        `
        return template
    }

    renderMenu() {
        const template = `
            <div class="row">
                <div class="col-md d-flex justify-content-center">
                    <button type="button" class="btn btn-primary">
                        <span class="spinner-border spinner-border-sm" style="display: none" role="status" aria-hidden="true"></span>
                        Сохранить
                    </button>
                </div>
            </div>
        `
        return template
    }

    /**
     * 
     * @param {array} content
     */
    renderContent(content) {
        let template = `
            <div class="tab-content" id="settings-contacts-content">
            <div
                class="tab-pane fade show active"
                id="${this.tabContentId}"
                role="tabpanel"
                aria-labelledby="settings-tabs-contacts"
              >`

        for (const item of content)
            template += item

        template += '</div></div>'

        return template
    }
}