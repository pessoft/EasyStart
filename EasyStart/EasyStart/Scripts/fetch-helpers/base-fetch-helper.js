class BaseFetchHelper extends Disposable {
    constructor() {
        super()

        this.fetchHelper = new FetchHelper()
        this.abortStore = {}
    }

    /**
     *
     * @param {object} body { props }
     * @param {object} options { url: string, isUploadFetch: bool, abortKey: string, abortPrevious: bool, method: string }
     * @description if options.abortKey not set then abortKey = options.url
     */
    async fetch(body = {}, options = {}) {
        const { url = null, abortKey = null, isUploadFetch = false, abortPrevious = true, method = FetchHelper.method.POST } = options

        if (!url)
            throw new Error("URL is empty")

        const _abortKey = abortKey || url
        const abortController = _abortKey ? this.prepareAbortBeforeCallFetch(_abortKey, abortPrevious) : null
        const abortSignal = abortController ? abortController.signal : null

        const result = await this.fetchHelper.fetch(url, body, method, abortSignal, isUploadFetch)
        abortSignal && this.clearAbortController(_abortKey)

        return result
    }

    prepareAbortBeforeCallFetch = (abortKey, abortPrevious = true) => {
        const prevAbortController = this.getAbortController(abortKey)
        if (prevAbortController && abortPrevious)
            prevAbortController.abort()

        const abortController = this.setAbortController(abortKey)

        return abortController
    }

    setAbortController = abortKey => {
        const abortController = new AbortController()
        this.abortStore[abortKey] = abortController

        return abortController
    }

    getAbortController = abortKey => this.abortStore[abortKey]
    clearAbortController = abortKey => delete this.abortStore[abortKey]

    dispose() {
        if (this.abortStore) {
            for (const key in this.abortStore)
                this.abortStore[key].abort()
        }
    }
}