class SettingsPage {
    constructor() {
        this.logic = new SettingsPageLogic()
        this.view = new SettingsPageView()

        this.init()
    }

    async init() {
        this.render()
    }

    render() {
        this.view.render()
    }
}