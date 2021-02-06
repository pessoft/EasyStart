class MdbSelect {
    constructor(element) {
        this.cloneElement = element.cloneNode(true)
        this.select = element
        this.id = `${this.storeCssClass.wrapSelect}-${this.generateId(6)}`
    }

    storeCssClass = {
        wrapSelect: 'mdb-select-wrapper',
        initializedSelect: 'mdb-select-initialized',
    }

    init() {
        this.initImitationSelect()
        this.initInput()
    }

    initImitationSelect() {
        this.select.outerHTML = this.getControlTemplate()
        this.cloneElement.classList.add(this.storeCssClass.initializedSelect)
        this.select = document.getElementById(this.id)
        this.select.appendChild(this.cloneElement)
    }

    getControlTemplate() {
        return `
            <div id="${this.id}" class="${this.storeCssClass.wrapSelect}">
                <div class="form-outline">
                    ${this.getImitationSelect()}
                    ${this.getLabel()}
                    <span class="mdb-select-arrow"></span>
                </div>
            </div>`
    }

    getImitationSelect () {
        const placeholder = this.cloneElement.getAttribute('placeholder') || ''
        const disabled = this.cloneElement.disabled ? 'disabled' : ''
        const inputTemplate = `<input ${disabled} type="text" readonly class="form-control mdb-select-input" placeholder="${placeholder}"/>`

        return inputTemplate
    }

    getLabel() {
        const label = this.cloneElement.getAttribute('label')
        const labelTemplate = label ? 
            `<label class="form-label" for="address-street">${label}</label>` :
            ''

        return labelTemplate
    }

    initInput() {
        document.querySelectorAll(`#${this.id} .form-outline`).forEach(formOutline => {
            new mdb.Input(formOutline).init()
        })
    }

    generateId(length) {
        let result = ''
        const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789'
        const charactersLength = characters.length

        for (let i = 0; i < length; i++) {
            result += characters.charAt(Math.floor(Math.random() * charactersLength))
        }

        return result
    }
}