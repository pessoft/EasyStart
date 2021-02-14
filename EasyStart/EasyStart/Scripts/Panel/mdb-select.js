class MdbSelect {
    constructor(element, options = {}) {
        this.cloneElement = element.cloneNode(true)
        this.select = element
        this.id = `${this.storeCssClass.wrapSelect}-${this.generateId(6)}`
        this.dropdownId = `dropdown-${this.id}`
        this.dropdownContainerId = `dropdown-container-${this.id}`
        this.options = {
            height: 190, // max height
            ...options
        }
    }

    storeCssClass = {
        wrapSelect: 'mdb-select-wrapper',
        initializedSelect: 'mdb-select-initialized',
        dropdownOpen: 'open',
        dropDownContainer: 'mdb-select-dropdown-container'
    }

    init() {
        this.initImitationSelect()
        this.initInput()
        this.initEvents()
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

    getImitationSelect() {
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

    initEvents() {
        this.initOpenEvent()
        this.initCloseEvent()
    }

    initOpenEvent() {
        const select = document.getElementById(this.id)
        select.removeEventListener('click', this.selectOpenHandler)
        select.addEventListener('click', this.selectOpenHandler)
    }

    initCloseEvent() {
        const selectCloseHandlerName = `selectCloseHandler${this.id}`
        this[selectCloseHandlerName] = event => {
            const select = document.getElementById(this.dropdownId)

            if (!select)
                return

            select.classList.remove(this.storeCssClass.dropdownOpen)

            const removeActionAfterAnimationClose = () => {
                const selectContainer = document.getElementById(this.dropdownContainerId)
                selectContainer.remove()
            }
            setTimeout(removeActionAfterAnimationClose, 500)
        }

        document.removeEventListener('click', this[selectCloseHandlerName])
        document.addEventListener('click', this[selectCloseHandlerName])
    }

    selectOpenHandler = event => {
        event.stopPropagation()

        const width = event.currentTarget.offsetWidth
        const windowCoordinates = event.currentTarget.getBoundingClientRect()
        const top = windowCoordinates.y + event.currentTarget.offsetHeight
        const left = windowCoordinates.x
        const coordinates = {
            top,
            left,
            width,
            height: this.options.height
        }

        this.showSelectDropdown(coordinates)
    }

    showSelectDropdown(coordinates) {
        const dropdown = this.getSelectContainer(coordinates)
        document.body.appendChild(dropdown)

        setTimeout(this.openDropdown, 50)
    }

    openDropdown = () => {
        const dropdownList = document.getElementById(this.dropdownId)
        dropdownList.classList.add(this.storeCssClass.dropdownOpen)
    }

    getSelectContainer(coordinates) {
        const optionsTemplate = this.getOptionsWrapper(coordinates.height)
        const style = `
            position: absolute;
            width: ${coordinates.width}px;
            top: ${coordinates.top}px;
            left: ${coordinates.left}px;`
        const optionsWrapper = `
                <div tabindex="0" class="mdb-select-dropdown rounded" id="${this.dropdownId}">
                    ${optionsTemplate}
                </div>`

        const dropdown = document.createElement('div')
        dropdown.classList.add(this.storeCssClass.dropDownContainer)
        dropdown.style.cssText = style
        dropdown.innerHTML = optionsWrapper
        dropdown.id = this.dropdownContainerId

        return dropdown
    }

    getOptionsWrapper(height) {
        const optionsListTemplate = this.getOptionsList()
        const template = `<div class="mdb-select-options-wrapper" style="max-height: ${height}px; height: ${height}px;">${optionsListTemplate}</div>`

        return template
    }

    getOptionsList() {
        return ''
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