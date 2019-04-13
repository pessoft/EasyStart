
function successCallBack(func, loader) {
    return function (data) {
        func(data, loader);
    }
}