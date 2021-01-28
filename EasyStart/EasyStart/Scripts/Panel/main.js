document.addEventListener('DOMContentLoaded', main)

async function main() {
    const startup = new Startup()

    await startup.configure()
    startup.run()
    hidePreloader()
}

function hidePreloader() {
    document.querySelector('#preloader-app').style.display = 'none'
}