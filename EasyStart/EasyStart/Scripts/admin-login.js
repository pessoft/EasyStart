function login() {
    let loader = new Loader($("#login-form"));

    loader.start();
    let login = $("#login").val();
    let password = $("#password").val();
    let successFunc = (result) => {
        if (result.Success) {
            window.location.href = result.URL;
        } else {
            showErrorMessage(result.ErrorMessage);
            loader.stop();
        }
    }

    if (!login || !password) {
        showErrorMessage("Введите логин и пароль");
        loader.stop();
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