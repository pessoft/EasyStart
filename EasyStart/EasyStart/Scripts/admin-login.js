function login() {
    let login = $("#login").val();
    let password = $("#password").val();
    let successFunc = (result) => {
        if (result.Success) {
            window.location.href = result.URL;
        } else {
            showErrorMessage(result.ErrorMessage);
        }
    }

    if (!login || !password) {
        return;
    }

    let loginData = {
        Login: login,
        Password: password
    }

    $.post("/Home/Login", loginData, successCallBack(successFunc, null));
 }

function loginByEnter(event) {
    if (event.keyCode == 13) {
        login();
    }
}