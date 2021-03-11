class FetchHelper {
    async fetch(query, body, method = 'POST', abortSignal = null, isUploadFetch = false) {
        const options = this.getOption(body, method, abortSignal, isUploadFetch)

        try {
            const response = await fetch(query, options)

            if (response.ok) {
                const text = await response.text()
                let json = this.tryParseJson(text)

                if (json.isJson) {
                    if (json.object.success)
                        return json.object.data
                    else
                        throw new Error(json.object.errorMessage)
                }

                return
            }
        } catch (ex) {
            throw ex
        }

        throw new Error('Error API')
    }

    tryParseJson(str) {
        try {
            const object = JSON.parse(str)

            return {
                isJson: true,
                object
            }
        } catch (e) {
            return {
                isJson: false,
                object: null
            }
        }

        return true;
    }

    getOption(body, method, signal, isUploadFetch = false) {
        

        const options = {
            method: method,
            contentType: !isUploadFetch,
            processData: !isUploadFetch,
            signal,
            headers: {
                'Content-Type': 'application/json'
            }
        }

        if (method == FetchHelper.method.GET) {
            options.contentType = false
        } else
            options.body = JSON.stringify(body)
            
        return options
    }
}

FetchHelper.AbortError = 'AbortError'
FetchHelper.method = {
    GET: 'GET',
    POST: 'POST'
}