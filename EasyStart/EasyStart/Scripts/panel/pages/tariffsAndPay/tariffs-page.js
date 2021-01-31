class TariffsPage {
    constructor() {
        this.logic = new TariffsPageLogic()
        this.view = new TariffsPageView()

        this.init()
    }

    async init() {
        this.render()
    }

    render() {
        this.view.render()
    }
}