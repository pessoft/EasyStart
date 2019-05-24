
function successCallBack(func, loader) {
    return function (data) {
        if (func) {
            func(data, loader);
        }
    }
}

function generateRandomString() {
    return Math.random().toString(36).slice(-6);
}


function showInfoMessage(message) {
    $.notify(message, {
        position: "top left",
        className: "info"
    });
}

function showWarningMessage(message) {
    $.notify(message, {
        position: "top left",
        className: "warn"
    });
}

function showErrorMessage(message) {
    $.notify(message, {
        position: "top left",
        className: "error"
    });
}