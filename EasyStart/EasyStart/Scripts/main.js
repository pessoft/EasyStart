
function successCallBack(func, loader) {
    return function (data) {
        if (func) {
            func(data, loader);
        }
    }
}

function generateRandom() {
    return Math.random().toString(36).slice(-6);
}