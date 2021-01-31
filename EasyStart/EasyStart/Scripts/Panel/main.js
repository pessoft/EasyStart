document.addEventListener('DOMContentLoaded', main)

async function main() {
    const startup = new Startup()

    await startup.configure()
    startup.run()
}