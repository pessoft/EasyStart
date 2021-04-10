class MdbSelect {
    constructor(element, options = {}) {
        this.cloneElement = element.cloneNode(true)
        this.select = element
        this.id = `${this.storeCssClass.wrapSelect}-${this.generateId(6)}`
        this.dropdownId = `dropdown-${this.id}`
        this.dropdownContainerId = `dropdown-container-${this.id}`
        this.options = {
            height: 190, // max height
            optionHeight: 38,
            ...options,
            multiple: this.cloneElement.multiple
        }
    }

    storeCssClass = {
        wrapSelect: 'mdb-select-wrapper',
        initializedSelect: 'mdb-select-initialized',
        dropdownOpen: 'open',
        dropDownContainer: 'mdb-select-dropdown-container',
        selectInput: 'mdb-select-input',
        selectOption: 'mdb-select-option',
        focused: 'focused',
        activeLabel: 'active',
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
        const multiple = this.options.multiple ? 'multiple' : ''
        const iputId = this.generateId(6)
        return `
            <div id="${this.id}" class="${this.storeCssClass.wrapSelect}" ${multiple}>
                <div class="form-outline">
                    ${this.getImitationSelect(iputId)}
                    ${this.getLabel(iputId)}
                    <span class="mdb-select-arrow"></span>
                </div>
            </div>`
    }

    getImitationSelect(id) {
        const placeholder = this.cloneElement.getAttribute('placeholder') || ''
        const disabled = this.cloneElement.disabled ? 'disabled' : ''
        const inputTemplate = `<input id="${id}" ${disabled} type="text" readonly class="form-control ${this.storeCssClass.selectInput}" placeholder="${placeholder}"/>`

        return inputTemplate
    }

    getLabel(forId) {
        const label = this.cloneElement.getAttribute('label')
        const labelTemplate = label ?
            `<label class="form-label mdb-select-label" for="${forId}">${label}</label>` :
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
            const select = document.getElementById(this.id)
            const input = select.querySelector('input')
            input.classList.remove(this.storeCssClass.focused)

            const label = select.querySelector('label')
            label.classList.remove(this.storeCssClass.activeLabel)

            const dropDownList = document.getElementById(this.dropdownId)

            if (!dropDownList)
                return

            dropDownList.classList.remove(this.storeCssClass.dropdownOpen)

            const removeActionAfterAnimationClose = () => {
                const selectContainer = document.getElementById(this.dropdownContainerId)
                selectContainer.remove()
            }
            setTimeout(removeActionAfterAnimationClose, 500)
        }

        document.removeEventListener('click', this[selectCloseHandlerName])
        document.addEventListener('click', this[selectCloseHandlerName])
    }

    initSelectedOptionEvent() {
        const select = document.getElementById(this.id)
        const dropdownList = document.getElementById(this.dropdownId)
        const isMultiple = select.hasAttribute('multiple')
        const selectedHandler = isMultiple ? this.selectedMultipleOptionHandler : this.selectedOptionHandler
        const options = dropdownList.querySelectorAll(`.${this.storeCssClass.selectOption}`)

        dropdownList.addEventListener('click', selectedHandler)
        //options.forEach(p => p.addEventListener('click', selectedHandler))
    }

    getTargetOptionClick = e => {
        let isOptionClick = e.classList.contains(this.storeCssClass.selectOption)
        let targetElem = isOptionClick ? e : null

        if (!isOptionClick) {
            const parent = e.closest(`.${this.storeCssClass.selectOption}`)
            isOptionClick = parent != null
            targetElem = parent
        }

        return targetElem
    }

    selectedOptionHandler = event => {
        const el = this.getTargetOptionClick(event.target)

        if (el && el.classList.contains('disabled') || !el) {
            event.stopPropagation()
            return
        }

        const value = el.getAttribute('value')
    }

    selectedMultipleOptionHandler = event => {
        event.stopPropagation()

        const el = this.getTargetOptionClick(event.target)
        if (el && el.classList.contains('disabled') || !el)
            return

        const value = el.getAttribute('value')
    }

    selectOpenHandler = event => {
        event.stopPropagation()

        const dropDownList = document.getElementById(this.dropdownId)
        if (dropDownList) {
            //document.body.click()
            return
        }

        const select = document.getElementById(this.id)
        const input = select.querySelector('input')
        input.classList.add(this.storeCssClass.focused)

        const label = select.querySelector('label')
        label.classList.add(this.storeCssClass.activeLabel)

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
        this.initSelectedOptionEvent()

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
        const template = `<div class="mdb-select-options-wrapper" style="max-height: ${height}px;">${optionsListTemplate}</div>`

        return template
    }

    getOptionsList() {
        let optionsTemplate = ''
        const options = this.cloneElement.options

        for (let i = 0; i < options.length; ++i) {
            const item = options.item(i)
            const selected = item.selected ? 'selected active' : ''
            const disabled = item.disabled ? 'disabled' : ''
            const cbxTemplate = this.options.multiple ? `<input type="checkbox" class="form-check-input" ${disabled}>` : ''
            const template = `
                <div class="${this.storeCssClass.selectOption} ${selected} ${disabled}" style="height: ${this.options.optionHeight}px;" value="${item.value}">
                    <span class="mdb-select-option-text">${cbxTemplate}${item.text}</span>
                </div>
            `
            optionsTemplate += template
        }

        return optionsTemplate
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