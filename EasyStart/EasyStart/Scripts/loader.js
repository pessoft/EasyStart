class Loader {
    constructor(targetElement) {
        this.TargetElement = $(targetElement);
    }

    start() {
        let loader = `<div class="loader"><img src="../images/loader.gif"/><div>`
        let form = $(e).find("form");

        this.stop();
        this.TargetElement.append(loader);
    }

    stop() {
        this.TargetElement.find(".loader").remove();
    }
}