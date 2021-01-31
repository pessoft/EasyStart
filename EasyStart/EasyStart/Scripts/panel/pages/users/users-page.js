class UsersPage {
    constructor() {
        this.logic = new UsersPageLogic()
        this.view = new UsersPageView()

        this.init()
    }

    async init() {
        this.render()
    }

    render() {
        this.view.render()
    }
}