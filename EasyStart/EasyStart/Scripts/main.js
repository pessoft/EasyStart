
function successCallBack(func, loader) {
    return function (data) {
        func(data, loader);
    }
}

function GenerateRandom() {
    return Math.random().toString(36).slice(-6);
}