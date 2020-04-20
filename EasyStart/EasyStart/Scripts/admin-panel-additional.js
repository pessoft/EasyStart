const OrderToolType = {
    OrderAsk: 0,
    OrderDesc: 1,
    OrderApply: 2,
    OrderCancel: 3
}

const OrderType = {
    NewOrders: 0,
    HistoryOrders: 1
}

class OrderTools {
    constructor(options) {
        this.$container = $(`#${options.containerId}`)
        this.$orderContainer = this.getOrdersContainer(options.orderType)
        this.isActiveCssClass = 'active-tool-item'
        
        this.renderTools(options.tools)

    }

    getOrdersContainer(orderType) {
        if (orderType == OrderType.NewOrders)
            return $(`#order .order-list-grid`)
        else
            return $(`#history .order-list-grid`)
    }

    renderTools(tools) {
        const templates = []

        for (const tool of tools) {
            const template = this.getTemplateTool(
                tool.type,
                tool.isActive
            )

            if (template)
                templates.push(template)
        }

        this.$container.html(templates)
    }

    getTemplateTool(typeId, isActive = false) {
        let icon = ''
        const activeClass = isActive ? this.isActiveCssClass : ''

        switch (typeId) {
            case OrderToolType.OrderAsk:
                icon = '<i class="fal fa-sort-numeric-down"></i>'
                break
            case OrderToolType.OrderDesc:
                icon = '<i class="fal fa-sort-numeric-up"></i>'
                break
            case OrderToolType.OrderApply:
                icon = '<i class="fal fa-check-double sm-font-size"></i>'
                break
            case OrderToolType.OrderCancel:
                icon = '<i class="fal fa-trash-alt sm-font-size"></i>'
                break
            default:
                icon = ''
                break;
        }

        const btnTemplate = icon ?
            `
                <button class="simple-text-btn order-tool-btn ${activeClass}" order-type="${typeId}">
                  ${icon}
                </button>
            ` :
            ''

        const $btnTemplate = this.bindEvent(btnTemplate, typeId)

        return $btnTemplate
    }

    bindEvent(template, typeId) {
        const self = this
        const $template = template ? $(template) : ''

        $template && $template.unbind('click')

        switch (typeId) {
            case OrderToolType.OrderAsk:
            case OrderToolType.OrderDesc:
                $template.bind('click', function () {
                    self.orderReverse(this)
                })
                break
            case OrderToolType.OrderApply:
                $template.bind('click', function () {
                    self.toggleApplyOrders(this)
                })
                break
            case OrderToolType.OrderCancel:
                $template.bind('click', function () {
                    self.toggleCancelOrders(this)
                })
                break
        }

        return $template
    }

    orderReverse(e) {
        const $e = $(e)
        const $conteiner = this.$orderContainer

        $conteiner.children().each(function (i, item) {
            $conteiner.prepend(item)
        })

        const newActiveOrderType = parseInt($e.attr('order-type'))
        const lastOrderType = OrderToolType.OrderAsk == newActiveOrderType ? 
            OrderToolType.OrderDesc :
            OrderToolType.OrderAsk
            

        $e.addClass(this.isActiveCssClass)
        this.$container.
            find(`button[order-type=${lastOrderType}]`).
            removeClass(this.isActiveCssClass)
    }

    toggleApplyOrders = e => {
        this.toggleOrdersByStatus(e, OrderStatus.Processed)
    }

    toggleCancelOrders = e => {
        this.toggleOrdersByStatus(e, OrderStatus.Cancellation)
    }

    toggleOrdersByStatus(e, statusId) {
        const $e = $(e)
        const $conteiner = this.$orderContainer

        $e.toggleClass(this.isActiveCssClass)
        const isActive = $e.hasClass(this.isActiveCssClass)
        const animationDuration = 500
        const $orderCards = $conteiner.find(`.order-item-grid[status-id=${statusId}]`)

        if (isActive)
            $orderCards.show(animationDuration)
        else
            $orderCards.hide(animationDuration)
    }
}

//бодрим сервер раз в 2 минуту
//что бы не уснул
function cheerupServer() {
    setInterval(
        () => $.post('/Admin/CheerupServer'),
        1000 * 20
    )
}