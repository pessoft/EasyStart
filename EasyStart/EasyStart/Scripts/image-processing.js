const AspectRatioType = {
    square: 1 / 1,
    news: 115 / 37
}

var ImageProcessingInstance = null

class ImageProcessing {
    constructor(input, actionSuccess, aspectRatio, compressionQuality = 0.6) {
        this.file = input.files[0]
        this.actionSuccess = actionSuccess
        this.aspectRatio = aspectRatio
        this.quality = compressionQuality
        this.input = input
        this.cropper = null
        this.loader = null
        this.showDialogImageProcessing()
    }

    imageProcessingDialogId = 'imageProcessingDialog'
    imageProcessingId = 'image-processing'

    showDialogImageProcessing = () => {
        const $dialg = $(`#${this.imageProcessingDialogId}`)

        this.setImage()
        Dialog.showModal($dialg)
    }

    setImage = () => {
        const reader = new FileReader();
        const self = this

        reader.onload = function (e) {
            const $image = $(`#${self.imageProcessingId}`)

            $image.attr("src", e.target.result)

            self.cropper = new Cropper($image[0], {
                viewMode: 1,
                aspectRatio: self.aspectRatio,
                autoCropArea: 1,
            })
        }

        reader.readAsDataURL(this.file)
    }

    startProcessing = () => {
        this.loader = new Loader($(`#${this.imageProcessingDialogId}`))
        this.loader.start()

        this.cropImage()
        this.compressImage()
    }

    cropImage = () => {
        if (!this.actionSuccess)
            return

        this.cropper.crop()
    }

    compressImage = () => {
        if (!this.actionSuccess)
            return

        const imageType = this.getImageType()
        const self = this

        const compression = blob => {
            
            let fileName = generateRandomString(20).replace(/\./g, '') + `.${imageType.type}`
            let newImage = new File([blob], fileName, {
                type: imageType.mimeType,
            })

            new Compressor(newImage, {
                quality: this.quality,
                success(result) {
                    self.uploadImage(result)
                }
            })
        }

        const canvas = this.cropper.getCroppedCanvas()
        canvas.toBlob(compression, imageType.mimeType)
    }

    getImageType = () => {
        let imageName = this.file.name
        const imageType = imageName.substring(imageName.lastIndexOf('.') + 1).toLowerCase()

        return {
            type: imageType == 'png' ? 'png' : 'jpg',
            mimeType: imageType == 'png' ? 'image/png' : 'image/jpeg'
        }
    }

    closeDialogImageProcessing = () => {
        this.destroy()
        Dialog.close($(`#${this.imageProcessingDialogId}`))
    }

    destroy = () => {
        if (this.cropper)
            this.cropper.destroy()

        $(this.input).val('')
    }


    uploadImage = image => {
        const self = this
        const errMsg = 'При сохранении изображения что-то пошло не так...'

        const dataImage = new FormData();
        dataImage.append('file', image, image.name);

        $.ajax({
            type: 'POST',
            url: '/Admin/UploadImage',
            contentType: false,
            processData: false,
            data: dataImage,
            success: function (data) {
                if (data.Success)
                    self.actionSuccess(data.URL)
                else
                    showErrorMessage(errMsg)

                self.loader.stop()
                self.closeDialogImageProcessing()
            },
            error: function () {
                showErrorMessage(errMsg)
                self.closeDialogImageProcessing()
            }
        });
    }
}

function startImageprocessing() {
    if (ImageProcessingInstance)
        ImageProcessingInstance.startProcessing()
}

function closeImageprocessingDialog() {
    if (ImageProcessingInstance)
        ImageProcessingInstance.closeDialogImageProcessing()
}