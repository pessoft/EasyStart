class EventListener {
    constructor() {
        this.listeners = {}
    }

    addEventListener(eventType, handler) {
        if (!eventType || !handler)
            return

        const handlers = this.listeners[eventType] || []
        handlers.push(handler)

        this.listeners[eventType] = handlers
    }

    removeEventListener(eventType, removeHandler) {
        if (!eventType)
            return

        if (!handler)
            delete this.listeners[eventType]
        else {
            const indexHandler = this.listeners[eventType].findIndex(handler => handler == removeHandler)

            if (indexHandler != -1) {
                this.listeners.splice(indexHandler, 1)
            }
        }
    }

    dispatchEvent(eventType, payload) {
        if (!eventType)
            return

        this.listeners[eventType].forEach(handler => handler(payload))
    }
}