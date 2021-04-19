﻿class MdbSelect {
    constructor(element, options = {}) {
        this.cloneElement = element.cloneNode(true)
        this.select = element

        const baseId = `${this.storeCssClass.wrapSelect}-${this.generateId(6)}`
        this.storeIds = {
            id: baseId,
            dropdownId: `dropdown-${baseId}`,
            dropdownContainerId: `dropdown-container-${baseId}`,
            inputSelectId: this.generateId(6),
            inputSelectWrapperId: this.generateId(6),
            inputSerachId: this.generateId(6)
        }

        this.options = {
            height: 190, // max height
            optionHeight: 38,
            ...options,
            multiple: this.cloneElement.multiple
        }
    }

    storeCssClass = {
        wrapSelect: 'mdb-select-wrapper',
        selectOptionsWrapper: 'mdb-select-options-wrapper',
        initializedSelect: 'mdb-select-initialized',
        dropdownOpen: 'open',
        dropDownContainer: 'mdb-select-dropdown-container',
        selectInput: 'mdb-select-input',
        selectOption: 'mdb-select-option',
        focused: 'focused',
        activeLabel: 'active',
        selectedOption: 'selected'
    }

    init() {
        this.initImitationSelect()
        this.initInput()
        this.initEvents()
    }

    initImitationSelect() {
        this.select.outerHTML = this.getControlTemplate()
        this.cloneElement.classList.add(this.storeCssClass.initializedSelect)
        this.select = document.getElementById(this.storeIds.id)
        this.select.appendChild(this.cloneElement)
    }

    getControlTemplate() {
        const multiple = this.options.multiple ? 'multiple' : ''
        const iputId = this.generateId(6)
        return `
            <div id="${this.storeIds.id}" class="${this.storeCssClass.wrapSelect}" ${multiple}>
                <div id="${this.storeIds.inputSelectWrapperId}" class="form-outline">
                    ${this.getImitationSelect()}
                    ${this.getLabel()}
                    <span class="mdb-select-arrow"></span>
                </div>
            </div>`
    }

    getImitationSelect() {
        const placeholder = this.cloneElement.getAttribute('placeholder') || ''
        const disabled = this.cloneElement.disabled ? 'disabled' : ''
        const value = [...this.cloneElement.querySelectorAll('option')].
            filter(p => p.selected).
            map(p => p.text).
            join(', ')

        const inputTemplate = `
            <input id="${this.storeIds.inputSelectId}" ${disabled}
                type="text"
                readonly
                value="${value}"
                class="form-control
                ${this.storeCssClass.selectInput}"
                placeholder="${placeholder}"/>`

        return inputTemplate
    }

    getLabel() {
        const label = this.cloneElement.getAttribute('label')
        const labelTemplate = label ?
            `<label class="form-label mdb-select-label" for="${this.storeIds.inputSelectId}">${label}</label>` :
            ''

        return labelTemplate
    }

    initInput = () => {
        const input = document.getElementById(this.storeIds.inputSelectWrapperId)
        new mdb.Input(input).init()
    }

    updateInput = () => {
        const input = document.getElementById(this.storeIds.inputSelectWrapperId)
        const instance = mdb.Input.getInstance(input)
        instance && instance.update()
    }

    initEvents() {
        this.initOpenEvent()
        this.initCloseEvent()
    }

    initOpenEvent() {
        const select = document.getElementById(this.storeIds.id)
        select.removeEventListener('click', this.selectOpenHandler)
        select.addEventListener('click', this.selectOpenHandler)
    }

    initCloseEvent() {
        const selectCloseHandlerName = `selectCloseHandler${this.storeIds.id}`
        this[selectCloseHandlerName] = event => {
            const select = document.getElementById(this.storeIds.id)
            const input = select.querySelector('input')
            input.classList.remove(this.storeCssClass.focused)

            const label = select.querySelector('label')
            label.classList.remove(this.storeCssClass.activeLabel)

            const dropDownList = document.getElementById(this.storeIds.dropdownId)

            if (!dropDownList)
                return

            dropDownList.classList.remove(this.storeCssClass.dropdownOpen)

            const removeActionAfterAnimationClose = () => {
                const selectContainer = document.getElementById(this.storeIds.dropdownContainerId)
                selectContainer && selectContainer.remove()
            }
            setTimeout(removeActionAfterAnimationClose, 500)
        }

        document.removeEventListener('click', this[selectCloseHandlerName])
        document.addEventListener('click', this[selectCloseHandlerName])
    }

    initSelectedOptionEvent() {
        const dropdownList = document.getElementById(this.storeIds.dropdownId)
        dropdownList.addEventListener('click', this.selectedOptionHandler)
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
        if (this.options.multiple)
            event.stopPropagation()

        const el = this.getTargetOptionClick(event.target)
        if (el && el.classList.contains('disabled') || !el) {
            event.stopPropagation()
            return
        }

        const value = el.getAttribute('value')
        const originOption = this.cloneElement.querySelector(`option[value='${value}']`)
        const isOriginSelected = originOption.selected

        if (this.options.multiple)
            originOption.selected = !isOriginSelected
        else
            originOption.selected = true

        const toggleCheckedCbxMultiple = checked => el.querySelector('input[type=checkbox]').checked = checked
        if (isOriginSelected) {
            el.classList.remove(this.storeCssClass.selectedOption)
            this.options.multiple && toggleCheckedCbxMultiple(false)
        } else {
            el.classList.add(this.storeCssClass.selectedOption)
            this.options.multiple && toggleCheckedCbxMultiple(true)
        }

        const updateInputSelectValue = oldValue => {
            if (!this.options.multiple)
                return originOption.text

            const values = oldValue.split(',').map(p => p.trim()).filter(p => !!p)

            if (isOriginSelected) {
                const index = values.findIndex(p => p == originOption.text)

                if (index != -1)
                    values.splice(index, 1)
            } else {
                values.push(originOption.text)
            }

            return values.join(', ')
        }

        const inputSelect = document.getElementById(this.storeIds.inputSelectId)
        inputSelect.value = updateInputSelectValue(inputSelect.value)

        this.updateInput()
    }

    selectOpenHandler = event => {
        event.stopPropagation()

        const dropDownList = document.getElementById(this.storeIds.dropdownId)
        if (dropDownList) {
            document.body.click()
            return
        }

        const select = document.getElementById(this.storeIds.id)
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
        this.configInputSearch()
    }

    configInputSearch() {
        const searchInput = document.getElementById(this.inputSerachId)

        if (searchInput) {
            searchInput.focus()
            searchInput.addEventListener('input', this.searchInputHandler)
        }
    }

    searchInputHandler = event => {
        const searchText = event.currentTarget.value.trim().toLowerCase()
        const newOptionsWrapperContent = this.getOptionsWrapperContent(searchText)
        const dropdwonList = document.getElementById(this.storeIds.dropdownContainerId)
        const optionsWrapper = dropdwonList.querySelector(`.${this.storeCssClass.selectOptionsWrapper}`)

        optionsWrapper.innerHTML = newOptionsWrapperContent
        this.initSelectedOptionEvent()
    }

    showSelectDropdown(coordinates) {
        const dropdown = this.getSelectContainer(coordinates)
        document.body.appendChild(dropdown)
        this.initSelectedOptionEvent()

        setTimeout(this.openDropdown, 50)
    }

    openDropdown = () => {
        const dropdownList = document.getElementById(this.storeIds.dropdownId)
        dropdownList.classList.add(this.storeCssClass.dropdownOpen)
    }

    getSelectContainer(coordinates) {
        const searchTemplate = this.getSearchBlock()
        const optionsTemplate = this.getOptionsWrapper(coordinates.height)
        const style = `
            position: absolute;
            width: ${coordinates.width}px;
            top: ${coordinates.top}px;
            left: ${coordinates.left}px;`
        const optionsWrapper = `
                <div tabindex="0" class="mdb-select-dropdown rounded" id="${this.storeIds.dropdownId}">
                    ${searchTemplate}
                    ${optionsTemplate}
                </div>`

        const dropdown = document.createElement('div')
        dropdown.classList.add(this.storeCssClass.dropDownContainer)
        dropdown.style.cssText = style
        dropdown.innerHTML = optionsWrapper
        dropdown.id = this.storeIds.dropdownContainerId

        return dropdown
    }

    getOptionsWrapper(height, searchText = null) {
        const content = this.getOptionsWrapperContent(searchText)
        const template = `<div class="${this.storeCssClass.selectOptionsWrapper}" style="max-height: ${height}px;">${content}</div>`

        return template
    }

    getOptionsWrapperContent(searchText = null) {
        const groups = this.cloneElement.querySelectorAll('optgroup')
        const options = this.cloneElement.options
        let content = ''
        if (groups && groups.length)
            content = this.getGroupList(groups, searchText)
        else
            content = this.getOptionsList(options, searchText)

        if (searchText && !content)
            content = `<div class="select-no-results" style="height: ${this.options.optionHeight}px">Поиск не дал результатов</div>`

        return content
    }

    getSearchBlock() {
        const isSearchEnable = this.cloneElement.hasAttribute('search')
        let template = ''

        if (isSearchEnable) {
            const placeholder = this.cloneElement.getAttribute('search-placeholder') || 'Поиск...'
            template = `
                <div class="input-group">
                    <input id="${this.inputSerachId}" class="form-control" placeholder="${placeholder}" role="searchbox" type="text">
                </div>
            `
        }

        return template
    }

    getGroupList(groups, searchText = null) {
        let optGroupsTemplate = ''

        for (const optgroup of groups) {
            const options = optgroup.querySelectorAll('option')
            const optionsTemplate = this.getOptionsList(options, searchText)

            if (searchText && !optionsTemplate)
                continue

            const groupId = this.generateId(10)
            optGroupsTemplate += `
                <div class="mdb-select-option-group" role="group" id="${groupId}">
                    <label class="mdb-select-option-group-label" for="${groupId}" style="height: ${this.options.optionHeight}px;">
                        ${optgroup.label}
                    </label>
                    ${optionsTemplate}
                </div>`
        }

        return `<div class="mdb-select-options-list">${optGroupsTemplate}</div>`
    }

    getOptionsList(options, searchText = null) {
        let optionsTemplate = ''

        for (let i = 0; i < options.length; ++i) {
            const item = options.item(i)
            if (searchText) {
                const itemText = item.text.trim().toLowerCase()

                if (!itemText.includes(searchText))
                    continue
            }

            const selected = item.selected ? `${this.storeCssClass.selectedOption}` : ''
            const checked = item.selected ? 'checked' : ''
            const disabled = item.disabled ? 'disabled' : ''
            const cbxTemplate = this.options.multiple ? `<input type="checkbox" class="form-check-input" ${disabled} ${checked}>` : ''
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