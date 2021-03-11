class Loader {
    constructor(targetElement) {
        this.TargetElement = $(targetElement);
    }

    start(customClass = '') {
        let loader = `<div class="loader ${customClass}"><img src="../images/loader.gif"/><div>`

        this.stop();
        this.TargetElement.append(loader);
    }

    stop() {
        this.TargetElement.find(".loader").remove();
    }
}