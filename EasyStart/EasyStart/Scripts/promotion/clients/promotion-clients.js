class PromotionClients {
    constructor() {
        this.view = new PromotionClientsView()
        this.logic = new PromotionClientsLogic()

        this.logic.getClients()
    }
}