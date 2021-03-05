class CustomEventListener {
    constructor() {
        //key: event type
        //value: [handlers]
        this.listeners = {}
    }

    addEventListener(eventType, handler) {
        const eventListeners = this.listeners[eventType]

        if (eventListeners) 
            eventListeners.push(handler)
        else 
            this.listeners[eventType] = [handler]
    }

    removeEventListener(eventType, handler) {
        const eventListeners = this.listeners[eventType]

        if (eventListeners) {
            const index = eventListeners.findIdex(p => p == handler)
            eventListeners.splice(index, 1)

            if (eventListeners.length == 0)
                delete this.listeners[eventType]
        }
    }

    dispatchEvent(eventType, payload) {
        const eventListeners = this.listeners[eventType]

        if (eventListeners)
            eventListeners.forEach(handler => handler(payload))
    }
}

CustomEventListener.events = {
    promotionClients: {
        FIND_BY_PHONE: 'FIND_BY_PHONE'
    }
}